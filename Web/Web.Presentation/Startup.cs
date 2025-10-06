// <copyright file="Startup.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Web.Application.Extensions;
using Web.Infrastructure.Extensions;
using Web.Presentation.Filters;
using Web.Presentation.Middleware.Users;

/// <summary>
/// Provides a class that configures services and the HTTP request pipeline.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">
    /// Specififes an <see cref="IConfiguration"/> that represents the application configuration.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="configuration"/> is null.
    /// </exception>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <value>
    /// The configuration.
    /// </value>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="application">
    /// The application builder.
    /// </param>
    /// <param name="environment">
    /// The web host environment.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="application"/> or <paramref name="environment"/> is null.
    /// </exception>
    public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(environment);

        if (!environment.IsDevelopment())
        {
            application.UseHsts();
        }

        application.UseHangfireDashboard();

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

        application.UseEndpoints(x =>
        {
            x.MapControllers();
            x.MapHangfireDashboard();
            x.MapRazorPages();
        });
    }

    /// <summary>
    /// Configures services for the application.
    /// </summary>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="services"/> is null.
    /// </exception>
    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<ApiBehaviorOptions>(x => x.SuppressModelStateInvalidFilter = true);

        services.AddLogging();

        services.AddControllersWithViews(x =>
        {
            x.Filters.Add<UnhandledExceptionFilterAttribute>();
            x.Filters.Add<InvalidModelStateExceptionFilterAttribute>();
            x.Filters.Add<ValidationExceptionFilterAttribute>();
            x.Filters.Add<ForbiddenAccessExceptionFilterAttribute>();
            x.Filters.Add<EntityNotFoundExceptionFilterAttribute>();
            x.Filters.Add<ConflictExceptionFilterAttribute>();
            x.Filters.Add<BadRequestExceptionFilterAttribute>();
            x.Filters.Add<UnauthorizedExceptionFilterAttribute>();
        });

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

        services.AddFluentValidationAutoValidation();

        services.AddApplicationServices(this.Configuration);
        services.AddInfrastructureServices(this.Configuration);
    }
}
