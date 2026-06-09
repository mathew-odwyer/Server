using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;
using Winterhaven.Common.Events;

namespace Winterhaven.Events.Extensions;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static IServiceCollection AddEventServices(this IServiceCollection services, IConfiguration configuration)
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
