using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Winterhaven.Gateway.Presentation.Services.Events;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Presentation.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddEventForwarder<TEventForwarder>(this IServiceCollection services)
        where TEventForwarder : EventForwarderBase
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<EventForwarderBase, TEventForwarder>();
        return services;
    }

    internal static IServiceCollection AddRpcSessionTarget<TRpcTarget>(this IServiceCollection services)
            where TRpcTarget : class, IRpcTarget
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRpcTarget, TRpcTarget>();
        return services;
    }
}
