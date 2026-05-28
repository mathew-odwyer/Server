using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Winterhaven.API.Presentation.Authentication;
using Winterhaven.API.Presentation.Filters;
using Winterhaven.API.Presentation.Mappings.Maps;
using Winterhaven.API.Presentation.Mappings.Players;
using Winterhaven.API.Presentation.Mappings.Users;
using Winterhaven.API.Presentation.Transformers;
using Winterhaven.API.Presentation.Transformers.Security;

namespace Winterhaven.API.Presentation.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"));

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = WinterhavenBearerDefaults.Name;
            x.DefaultChallengeScheme = WinterhavenBearerDefaults.Name;
        })
        .AddPolicyScheme(WinterhavenBearerDefaults.Name, "JWT or API Key", x => x.ForwardDefaultSelector = context => context.Request.Headers.ContainsKey("X-API-KEY")
                    ? WinterhavenBearerDefaults.ServerAuthenticationScheme
                    : JwtBearerDefaults.AuthenticationScheme)
        .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(WinterhavenBearerDefaults.ServerAuthenticationScheme, null)
        .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = configuration.GetValue<bool>("JwtOptions:ValidateLifetime"),
            ValidateIssuerSigningKey = configuration.GetValue<bool>("JwtOptions:ValidateIssuerSigningKey"),
            ValidateIssuer = configuration.GetValue<bool>("JwtOptions:ValidateIssuer"),
            ValidateAudience = configuration.GetValue<bool>("JwtOptions:ValidateAudience"),
            ValidIssuer = configuration["JwtOptions:Issuer"],
            ValidAudience = configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtOptions:Secret"]!)),
        });

        return services;
    }

    internal static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        return services;
    }

    internal static IServiceCollection AddApiControllersWithFilters(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiBehaviorOptions>(x => x.SuppressModelStateInvalidFilter = false);

        services.AddControllersWithViews(x =>
        {
            x.Filters.Add<UnhandledExceptionFilterAttribute>();
            x.Filters.Add<InvalidModelStateActionFilterAttribute>();
            x.Filters.Add<ValidationExceptionFilterAttribute>();
            x.Filters.Add<ForbiddenAccessExceptionFilterAttribute>();
            x.Filters.Add<ResourceNotFoundExceptionFilterAttribute>();
            x.Filters.Add<ConflictExceptionFilterAttribute>();
            x.Filters.Add<UnauthorizedExceptionFilterAttribute>();
            x.Filters.Add<AcceptCaseActionFilterAttribute>();
        })
        .AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower);

        return services;
    }

    internal static IServiceCollection AddApiMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(x =>
        {
            x.AddProfile<UserMapper>();
            x.AddProfile<PlayerMapper>();
            x.AddProfile<MapMapper>();
        });

        return services;
    }

    internal static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();

        string version = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "unknown";

        services.AddOpenApi($"v{version}", x => x
                .AddDocumentTransformer<WinterhavenTransformer>()
                .AddDocumentTransformer<BearerSecuritySchemeTransformer>()
                .AddDocumentTransformer<ApiKeySecuritySchemeTransformer>());

        return services;
    }
}