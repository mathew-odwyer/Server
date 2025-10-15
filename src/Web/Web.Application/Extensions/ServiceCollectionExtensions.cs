// <copyright file="ServiceCollectionExtensions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Extensions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Web.Application.Behaviours;
using Web.Application.Options.Security;
using Web.Application.Profiles.Players;
using Web.Application.Profiles.Users;

/// <summary>
/// Provides extension methods for configuring and adding application services to an <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the specified <see cref="IServiceCollection"/> and configures them based on the provided configuration.
    /// </summary>
    /// <param name="services">
    /// Specifies an <see cref="IServiceCollection"/> representing the service collection to add the services to.
    /// </param>
    /// <param name="configuration">
    /// Specifies an <see cref="IConfiguration"/> representing the configuration to use for validating and configuring options.
    /// </param>
    /// <returns>
    /// Returns the <see cref="IServiceCollection"/> with the services added.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(x =>
        {
            x.AddProfile<UserProfile>();
            x.AddProfile<PlayerProfile>();
        });

        services.AddMediatR(x =>
        {
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddValidatedOptions<JwtOptions>(configuration);
        services.AddValidatedOptions<ClientTokenOptions>(configuration);

        return services;
    }

    /// <summary>
    /// Adds and configures options with validation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    /// The type of options to be configured.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to extend.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance to use for configuration.
    /// </param>
    /// <returns>
    /// Returns an <see cref="OptionsBuilder{TOptions}"/> instance for further configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <c>null</c>.
    /// </exception>
    public static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(typeof(TOptions).Name))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
