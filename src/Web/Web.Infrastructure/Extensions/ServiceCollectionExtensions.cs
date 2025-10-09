// <copyright file="ServiceCollectionExtensions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Extensions;

using FluentEmail.MailKitSmtp;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Application.Contexts;
using Web.Application.Contexts.Players;
using Web.Application.Contexts.Users;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;
using Web.Infrastructure.Contexts;
using Web.Infrastructure.Contexts.Players;
using Web.Infrastructure.Contexts.Users;
using Web.Infrastructure.Services.Users;

/// <summary>
/// Provides extension methods for configuring infrastructure services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">
    /// Specifies an <see cref="IServiceCollection"/> that represents to which the services are added.
    /// </param>
    /// <param name="configuration">
    /// Specifies an <see cref="IConfiguration"/> that represents the service used to retrieve configuration settings.
    /// </param>
    /// <returns>
    /// Returns the updated <see cref="IServiceCollection"/> with infrastructure services added.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> parameters are <c>null</c>.
    /// </exception>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddIdentityApiEndpoints<UserAccount>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(x =>
        {
            x.User.RequireUniqueEmail = true;
            x.SignIn.RequireConfirmedEmail = false;
        });

        services
            .AddDbContext<DbContext, DatabaseContext>(x => x.UseSqlServer(configuration.GetConnectionString("Default"), o =>
            {
                o.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }), ServiceLifetime.Scoped);

        services
            .AddHangfire(x => x
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage());

        services.AddHangfireServer();

        services
            .AddFluentEmail(configuration.GetValue<string>("SmtpOptions:DefaultFromEmail"), configuration.GetValue<string>("SmtpOptions:DefaultFromName"))
            .AddRazorRenderer()
            .AddMailKitSender(new SmtpClientOptions()
            {
                Server = configuration.GetValue<string>("SmtpOptions:Server"),
                Port = configuration.GetValue<int>("SmtpOptions:Port"),
                User = configuration.GetValue<string>("SmtpOptions:User"),
                Password = configuration.GetValue<string>("SmtpOptions:Password"),
                UseSsl = configuration.GetValue<bool>("SmtpOptions:UseSsl"),
                RequiresAuthentication = configuration.GetValue<bool>("SmtpOptions:RequiresAuthentication"),
            });

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IUserSessionTokenRepository, UserSessionTokenRepository>();

        services.AddScoped<IPlayerRepository, PlayerRepository>();

        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IUserAccountTokenService, UserAccountTokenService>();

        return services;
    }
}
