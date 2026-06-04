using Microsoft.Extensions.DependencyInjection;

namespace Winterhaven.Brokering.NATS.Extensions;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services">
    /// </param>
    /// <returns>
    /// </returns>
    public static IServiceCollection AddBrokeringServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IMessageBus, NatsMessageBus>();

        return services;
    }
}
