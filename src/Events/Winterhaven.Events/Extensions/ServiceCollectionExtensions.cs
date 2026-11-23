using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;
using Winterhaven.Common.Events;

namespace Winterhaven.Events.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddNats(1, x => x with
        {
            Name = "winterhaven-nats",
            Url = configuration["NATSOPTIONS__BASEURL"] + configuration["NATSOPTIONS__PORT"],
            SerializerRegistry = NatsJsonSerializerRegistry.Default,
        });

        services.AddSingleton<IMessageBus, NatsMessageBus>();

        return services;
    }
}
