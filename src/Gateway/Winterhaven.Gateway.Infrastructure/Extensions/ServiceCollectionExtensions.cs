using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Winterhaven.Common.Extensions;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Infrastructure.Options.Client;
using Winterhaven.Gateway.Infrastructure.Pipeline.Handlers;

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

        services.AddTransient<LoggingHandler>();
        services.AddTransient<ApiExceptionHandler>();

        services.AddGatewayClient<IUserAccountClient>("api/UserAccount");

        return services;
    }

    private static IServiceCollection AddGatewayClient<TClient>(this IServiceCollection services, string route)
        where TClient : class
    {
        services.AddRefitClient<TClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<ClientOptions>>();
                string version = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion ?? "unknown";

                client.BaseAddress = new Uri($"{settings.Value.BaseUrl}/{route}/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"Winterhaven Gateway/{version}");
            })
            .AddHttpMessageHandler<LoggingHandler>()
            .AddHttpMessageHandler<ApiExceptionHandler>();

        return services;
    }
}
