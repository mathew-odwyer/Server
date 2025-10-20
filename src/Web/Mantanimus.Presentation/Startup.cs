// <copyright file="Startup.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation;

using System.Text;
using System.Text.Json;
using Mantanimus.Core.Application.Extensions;
using Mantanimus.Infrastructure.Extensions;
using Mantanimus.Presentation.Factories;
using Mantanimus.Presentation.Filters;
using Mantanimus.Presentation.Middleware.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

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
            application.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mantanimus API v1"));
        }
        else
        {
            application.UseHsts();
        }

        application.UseRouting();

        application.UseAuthentication();
        application.UseMiddleware<UserSessionValidationMiddleware>();
        application.UseAuthorization();

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
        services.AddInfrastructureServices(this.Configuration, "Local");

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Mantanimus API",
                Version = "v1",
                Description = "API documentation for Mantanimus."
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
