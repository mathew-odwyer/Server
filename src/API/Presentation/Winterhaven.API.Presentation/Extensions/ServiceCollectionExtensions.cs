namespace Winterhaven.API.Presentation.Extensions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Winterhaven.API.Presentation.Authentication;
using Winterhaven.API.Presentation.Filters;
using Winterhaven.API.Presentation.Mappings.Maps;
using Winterhaven.API.Presentation.Mappings.Players;
using Winterhaven.API.Presentation.Mappings.Users;
using Winterhaven.API.Presentation.Transformers;
using Winterhaven.API.Presentation.Transformers.Security;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddWinterhavenControllersWithFilters(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiBehaviorOptions>(x => x.SuppressModelStateInvalidFilter = false);

        services.AddControllersWithViews(x =>
        {
            x.Filters.Add<UnhandledExceptionFilterAttribute>();
            x.Filters.Add<InvalidModelStateActionFilterAttribute>();
            x.Filters.Add<ValidationExceptionFilterAttribute>();
            x.Filters.Add<ForbiddenAccessExceptionFilterAttribute>();
            x.Filters.Add<EntityNotFoundExceptionFilterAttribute>();
            x.Filters.Add<ConflictExceptionFilterAttribute>();
            x.Filters.Add<UnauthorizedExceptionFilterAttribute>();
        });

        return services;
    }

    internal static IServiceCollection AddWinterhavenAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        return services;
    }

    internal static IServiceCollection AddWinterhavenApiServices(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();

        services.AddOpenApi("v0.3.0", options =>
        {
            options.AddDocumentTransformer<WinterhavenTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
        });

        return services;
    }

    internal static IServiceCollection AddWinterhavenAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = WinterhavenBearerDefaults.Name;
            x.DefaultChallengeScheme = WinterhavenBearerDefaults.Name;
        }).AddPolicyScheme(WinterhavenBearerDefaults.Name, "JWT or API Key", x =>
        {
            x.ForwardDefaultSelector = context =>
            {
                return context.Request.Headers.ContainsKey("X-API-KEY")
                    ? WinterhavenBearerDefaults.ServerAuthenticationScheme
                    : JwtBearerDefaults.AuthenticationScheme;
            };
        })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(WinterhavenBearerDefaults.ServerAuthenticationScheme, null)
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = configuration.GetValue<bool>("JwtOptions:ValidateLifetime"),
                    ValidateIssuerSigningKey = configuration.GetValue<bool>("JwtOptions:ValidateIssuerSigningKey"),
                    ValidateIssuer = configuration.GetValue<bool>("JwtOptions:ValidateIssuer"),
                    ValidateAudience = configuration.GetValue<bool>("JwtOptions:ValidateAudience"),
                    ValidIssuer = configuration["JwtOptions:Issuer"],
                    ValidAudience = configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtOptions:Secret"]!)),
                };
            });

        return services;
    }

    internal static IServiceCollection AddWinterhavenRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(x =>
        {
            x.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User?.FindFirst("username")?.Value
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions()
                    {
                        // Refresh request limit every X minutes.
                        AutoReplenishment = configuration.GetValue<bool?>("RateLimitOptions:AutoReplenishment") ?? true,

                        // Maximum of X requests per X minutes.
                        PermitLimit = configuration.GetValue<int?>("RateLimitOptions:PermitLimit") ?? 100,

                        // If the queue is full, reject the request.
                        QueueLimit = configuration.GetValue<int?>("RateLimitOptions:QueueLimit") ?? 10,

                        // Time window for rate limiting.
                        Window = TimeSpan.FromMinutes(configuration.GetValue<int?>("RateLimitOptions:WindowMinutes") ?? 1),
                    });
            });

            x.OnRejected = async (context, _) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RateLimiterOptions>>();

                logger.LogWarning("Rate limit rejected request for path: {Path}, User Data: {Identity}",
                    context.HttpContext.Request.Path,
                    context.HttpContext.User?.FindFirst("username")?.Value
                        ?? context.HttpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown");

                await Task.CompletedTask.ConfigureAwait(false);
            };
        });

        return services;
    }

    internal static IServiceCollection AddWinterhavenMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(x =>
        {
            x.AddProfile<UserMapper>();
            x.AddProfile<PlayerMapper>();
            x.AddProfile<MapMapper>();
        });

        return services;
    }
}