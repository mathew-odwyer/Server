using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Winterhaven.Common.Extensions;

/// <summary>
///   Provides common extension methods to an <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Adds and configures options with validation to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    ///   The type of options to be configured.
    /// </typeparam>
    /// <param name="services">
    ///   The <see cref="IServiceCollection"/> to extend.
    /// </param>
    /// <param name="configuration">
    ///   The <see cref="IConfiguration"/> instance to use for configuration.
    /// </param>
    /// <returns>
    ///   Returns an <see cref="OptionsBuilder{TOptions}"/> instance for further configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <c>null</c>.
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