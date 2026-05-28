using Microsoft.Extensions.DependencyInjection;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Presentation.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddRpcSessionTarget<TRpcTarget>(this IServiceCollection services)
        where TRpcTarget : class, IRpcTarget
    {
        services.AddScoped<IRpcTarget, TRpcTarget>();
        return services;
    }
}
