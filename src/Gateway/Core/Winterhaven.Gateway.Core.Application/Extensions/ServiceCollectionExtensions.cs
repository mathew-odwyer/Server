namespace Winterhaven.Gateway.Core.Application.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Winterhaven.Gateway.Core.Application.Services.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAccountService, UserAccountService>();

        return services;
    }
}
