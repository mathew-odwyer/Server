using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;
using Winterhaven.Gateway.Integrations.Services.Builders;
using Winterhaven.Gateway.Integrations.Services.Targets;
using Winterhaven.Gateway.Integrations.Services.Users;
using Winterhaven.Gateway.Presentation;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Integrations;

internal abstract class TestHostBase
{
    protected IHost Host { get; private set; }

    protected MockUserSessionContext UserSessionManager { get; } = new();

    protected async Task<WebSocketRpcConnection> CreateConnectionAsync(
        Action<WebSocketRpcConnectionBuilder> configure,
        string uri = "ws://localhost/ws",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);

        var builder = new WebSocketRpcConnectionBuilder();
        configure(builder);

        var client = Host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri(uri), cancellationToken).ConfigureAwait(false);

        return builder.Build(webSocket);
    }

    protected async Task SetUpTestHost()
    {
        var builder = new HostBuilder();

        builder.ConfigureWebHost(x =>
        {
            x.UseStartup<Startup>();
            x.UseTestServer();
            x.ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.Tests.json", optional: false));
            x.ConfigureTestServices(services =>
            {
                services.AddScoped<IRpcTarget, TestErrorRpcTarget>();
                services.AddScoped<IRpcTarget, TestAuthRpcTarget>();

                services.AddSingleton<IUserSessionContext>(UserSessionManager);
                services.AddSingleton<IUserSessionManager>(UserSessionManager);
            });
        });

        Host = builder.Build();
        await Host.StartAsync().ConfigureAwait(false);
    }

    protected async Task TearDownTestHost()
    {
        await Host.StopAsync().ConfigureAwait(false);
        Host.Dispose();
    }
}
