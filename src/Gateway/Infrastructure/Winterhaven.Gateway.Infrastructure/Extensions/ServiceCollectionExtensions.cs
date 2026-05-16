namespace Winterhaven.Gateway.Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO.Abstractions;
using Winterhaven.Common.Extensions;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Infrastructure.Clients.Users;
using Winterhaven.Gateway.Infrastructure.HTTP.Handlers;
using Winterhaven.Gateway.Infrastructure.Options;
using Winterhaven.Gateway.Infrastructure.Services.Sessions;
using Winterhaven.Gateway.Infrastructure.Services.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IFileSystem, FileSystem>();

        services.AddScoped<SessionContext>();
        services.AddScoped<ISessionContext>(sp => sp.GetRequiredService<SessionContext>());
        services.AddScoped<ISessionAuthenticator>(sp => sp.GetRequiredService<SessionContext>());

        services.AddTransient<LoggingHandler>();
        services.AddTransient<AccessTokenHandler>();
        services.AddTransient<ApiResponseHandler>();

        services.AddValidatedOptions<ClientOptions>(configuration);
        services.AddGatewayClient<IUserAccountClient, UserAccountClient>("api/UserAccount");

        services.AddScoped<IUserAccountService, UserAccountService>();

        return services;
    }

    private static IServiceCollection AddGatewayClient<TClient, TImplementation>(this IServiceCollection services, string route)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<ClientOptions>>().Value;

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Winterhaven Gateway/1.0");
            client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/" + route + "/");
        })
        .AddHttpMessageHandler<LoggingHandler>()
        .AddHttpMessageHandler<AccessTokenHandler>()
        .AddHttpMessageHandler<ApiResponseHandler>();

        return services;
    }
}