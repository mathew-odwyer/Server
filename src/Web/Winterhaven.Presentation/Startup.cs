// <copyright file="Startup.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Presentation;

using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Winterhaven.Core.Application.Extensions;
using Winterhaven.Infrastructure.Extensions;
using Winterhaven.Presentation.Factories;
using Winterhaven.Presentation.Filters;
using Winterhaven.Presentation.Middleware.Users;

internal sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IConfiguration Configuration { get; }

    public static void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(environment);

        if (environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Winterhaven API v1"));
        }
        else
        {
            application.UseHsts();
        }

        application.UseRouting();

        application.UseAuthentication();
        application.UseMiddleware<UserSessionValidationMiddleware>();
        application.UseAuthorization();

        application.UseRateLimiter();

        application.UseStaticFiles(new StaticFileOptions()
        {
            HttpsCompression = HttpsCompressionMode.DoNotCompress,
            ServeUnknownFileTypes = true,
            OnPrepareResponse = (x) =>
            {
                x.Context.Response.Headers.CacheControl = "no-cache, no-store";
                x.Context.Response.Headers.Pragma = "no-cache";
                x.Context.Response.Headers.Expires = "-1";
            },
        });

        application.UseEndpoints(x => x.MapControllers());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiBehaviorOptions>(x => x.SuppressModelStateInvalidFilter = true);
        services.AddFluentValidationAutoValidation(x => x.OverrideDefaultResultFactoryWith<InvalidModelStateActionResultFactory>());

        services.AddLogging();

        services.AddControllersWithViews(x =>
        {
            x.Filters.Add<UnhandledExceptionFilterAttribute>();
            x.Filters.Add<InvalidModelStateActionFilterAttribute>();
            x.Filters.Add<ValidationExceptionFilterAttribute>();
            x.Filters.Add<ForbiddenAccessExceptionFilterAttribute>();
            x.Filters.Add<EntityNotFoundExceptionFilterAttribute>();
            x.Filters.Add<ConflictExceptionFilterAttribute>();
            x.Filters.Add<UnauthorizedExceptionFilterAttribute>();
        })
        .AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower);

        services.AddRazorPages();

        services
            .AddAuthorization()
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = this.Configuration.GetValue<bool>("JwtOptions:ValidateLifetime"),
                    ValidateIssuerSigningKey = this.Configuration.GetValue<bool>("JwtOptions:ValidateIssuerSigningKey"),
                    ValidateIssuer = this.Configuration.GetValue<bool>("JwtOptions:ValidateIssuer"),
                    ValidateAudience = this.Configuration.GetValue<bool>("JwtOptions:ValidateAudience"),
                    ValidIssuer = this.Configuration["JwtOptions:Issuer"],
                    ValidAudience = this.Configuration["JwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JwtOptions:Secret"]!)),
                };
            });

        services.AddHttpContextAccessor();
        services.AddHealthChecks();

        services.AddApplicationServices(this.Configuration);
        services.AddInfrastructureServices(this.Configuration, "Docker");

        services.AddEndpointsApiExplorer();

        services.AddRateLimiter(x =>
        {
            x.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User?.FindFirst("username")?.Value
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions()
                    {
                        // Refresh request limit every X minutes.
                        AutoReplenishment = this.Configuration.GetValue<bool?>("RateLimitOptions:AutoReplenishment") ?? true,

                        // Maximum of X requests per X minutes.
                        PermitLimit = this.Configuration.GetValue<int?>("RateLimitOptions:PermitLimit") ?? 10,

                        // If the queue is full, reject the request.
                        QueueLimit = this.Configuration.GetValue<int?>("RateLimitOptions:QueueLimit") ?? 0,

                        // Time window for rate limiting.
                        Window = TimeSpan.FromMinutes(this.Configuration.GetValue<int?>("RateLimitOptions:WindowMinutes") ?? 1),
                    });
            });

            x.OnRejected = async (context, token) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();

                logger.LogWarning("Rate limit rejected request for path: {Path}, User Data: {IPAddress}",
                    context.HttpContext.Request.Path,
                    context.HttpContext.User?.FindFirst("username")?.Value
                        ?? context.HttpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown");

                await Task.CompletedTask.ConfigureAwait(false);
            };
        });

        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Winterhaven API",
                Version = "v1",
                Description = "API documentation for Winterhaven."
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    securityScheme,
                    Array.Empty<string>()
                }
            };

            x.AddSecurityDefinition("Bearer", securityScheme);
            x.AddSecurityRequirement(securityRequirement);
        });
    }
}
