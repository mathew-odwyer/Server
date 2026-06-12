using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;
using Refit;
using Winterhaven.Common;
using Winterhaven.Common.Extensions;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Chat;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Infrastructure.Options.Client;
using Winterhaven.Gateway.Infrastructure.Pipeline.Factories;
using Winterhaven.Gateway.Infrastructure.Pipeline.Handlers;
using Winterhaven.Gateway.Infrastructure.Services.Chat;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Infrastructure.Extensions;

/// <summary>
///   Provides extension methods for an <see cref="IServiceCollection"/> that registers the gateway infrastructure services.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Adds the gateway infrastructure services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">
    ///   The services.
    /// </param>
    /// <param name="configuration">
    ///   The configuration.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown if <paramref name="services"/> or <paramref name="configuration"/> parameter is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddGatewayInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddValidatedOptions<ClientOptions>(configuration);

        services.AddNats(1, x => x with
        {
            Name = "winterhaven-nats",
            Url = configuration["NATS_URL"] ?? "ws://nats:9222",
            SerializerRegistry = NatsJsonSerializerRegistry.Default,
        });

        services.AddScoped<UserSessionManager>();

        services.AddScoped<IUserSessionContext>(x => x.GetRequiredService<UserSessionManager>());
        services.AddScoped<IUserSessionManager>(x => x.GetRequiredService<UserSessionManager>());

        services.AddSingleton<IUserTokenParser, UserTokenParser>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IChatService, ChatService>();

        services.AddGatewayClient<IUserAccountClient>("api/UserAccount");

        services.AddSingleton<ApiExceptionFactory>();
        services.AddTransient<AccessTokenHandler>();

        return services;
    }

    private static IServiceCollection AddGatewayClient<TClient>(this IServiceCollection services, string route)
        where TClient : class
    {
        services.AddRefitClient<TClient>(x => new RefitSettings()
        {
            ExceptionFactory = x.GetRequiredService<ApiExceptionFactory>().CreateAsync,
        })
        .ConfigureHttpClient((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<ClientOptions>>();

            client.BaseAddress = new Uri($"{settings.Value.BaseUrl}/{route}");
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"Winterhaven Gateway/{BuildInformation.Version}");
        })
        .AddHttpMessageHandler<AccessTokenHandler>();

        return services;
    }
}
