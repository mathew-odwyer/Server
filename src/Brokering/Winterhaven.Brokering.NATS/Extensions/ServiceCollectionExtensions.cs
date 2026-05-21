namespace Winterhaven.Brokering.NATS.Extensions;

using global::NATS.Client.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Provides extension methods for registering brokering services to an <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Adds brokering services to the specified <paramref name="services"/>.
    /// </summary>
    /// <param name="services">
    ///   The <see cref="IServiceCollection"/> to register brokering services to.
    /// </param>
    /// <param name="configuration">
    ///   The <see cref="IConfiguration"/> used when registering the services.
    /// </param>
    /// <returns>
    ///   Returns the <see cref="IServiceCollection"/> with the services added.
    /// </returns>
    public static IServiceCollection AddBrokeringServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddNats(1, options => options with
        {
            Name = "winterhaven-nats",
            Url = configuration["NATS_URL"] ?? "ws://nats:9222",
        });

        return services;
    }
}