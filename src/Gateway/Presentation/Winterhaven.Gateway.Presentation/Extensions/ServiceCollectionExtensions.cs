namespace Winterhaven.Gateway.Presentation.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Winterhaven.Gateway.Presentation.Mappings.Users;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddGatewayMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(x =>
        {
            x.AddProfile<UserMapper>();
        });

        return services;
    }
}
