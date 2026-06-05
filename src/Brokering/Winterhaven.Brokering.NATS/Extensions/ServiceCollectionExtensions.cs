using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;

namespace Winterhaven.Brokering.NATS.Extensions;

/// <summary>
///   Provides extension methods for registering brokering services with an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Adds the necessary services for using the NATS message bus implementation to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">
    ///   The <see cref="IServiceCollection"/> to which the brokering services will be added.
    /// </param>
    /// <param name="configuration">
    ///   The <see cref="IConfiguration"/> instance that provides access to the application's configuration settings.
    /// </param>
    /// <returns>
    ///   The same <see cref="IServiceCollection"/> instance that was passed in, allowing for method chaining when configuring services.
    /// </returns>
    public static IServiceCollection AddBrokeringServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddNats(1, x => x with
        {
            Name = "winterhaven-nats",
            Url = configuration["NATS_URL"] ?? "ws://nats:9222",
            SerializerRegistry = NatsJsonSerializerRegistry.Default,
        });

        services.AddSingleton<IMessageBus, NatsMessageBus>();

        return services;
    }
}
