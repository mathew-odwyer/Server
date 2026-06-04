using System;
using Microsoft.Extensions.DependencyInjection;

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
    /// <returns>
    ///   The same <see cref="IServiceCollection"/> instance that was passed in, allowing for method chaining when configuring services.
    /// </returns>
    public static IServiceCollection AddBrokeringServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IMessageBus, NatsMessageBus>();

        return services;
    }
}
