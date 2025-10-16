// <copyright file="ServiceCollectionExtensions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Extensions;

using Mantanimus.Core.Application.Work;
using Mantanimus.Infrastructure.Work;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    /// <param name="connectionName">
    /// Specifies a <see cref="string"/> that represents the name of the connection string to be used when configuring the database context.
    /// </param>
    /// <returns>
    /// Returns the updated <see cref="IServiceCollection"/> with infrastructure services added.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/>, <paramref name="configuration"/> or <paramref name="connectionName"/> parameters are <c>null</c>.
    /// </exception>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string connectionName = "Docker")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionName);

        services
            .AddIdentityCore<IdentityUser<Guid>>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddSignInManager<SignInManager<IdentityUser<Guid>>>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(x =>
        {
            x.User.RequireUniqueEmail = true;
            x.SignIn.RequireConfirmedEmail = false;
        });

        services
            .AddDbContext<DbContext, DatabaseContext>(x => x.UseSqlServer(configuration.GetConnectionString(connectionName), o =>
            {
                o.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }), ServiceLifetime.Scoped);

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return services;
    }
}
