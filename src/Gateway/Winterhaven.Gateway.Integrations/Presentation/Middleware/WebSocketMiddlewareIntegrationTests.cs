using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Winterhaven.Gateway.Presentation;

namespace Winterhaven.Gateway.Integrations.Presentation.Middleware;

[TestFixture]
internal sealed class WebSocketMiddlewareIntegrationTests
{
    private IHost host;

    [Test]
    public async Task InvokeAsyncShouldAcceptWebSocketWhenRequestIsWebSocket()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();

        // Act
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        // Assert
        Assert.That(webSocket.State, Is.EqualTo(WebSocketState.Open));
    }

    [SetUp]
    public async Task Setup()
    {
        var builder = new HostBuilder();

        builder.ConfigureWebHost(x =>
        {
            x.UseTestServer();
            x.UseStartup<Startup>();
        });

        host = builder.Build();

        await host.StartAsync().ConfigureAwait(false);
    }

    [TearDown]
    public async Task TearDown()
    {
        await host.StopAsync().ConfigureAwait(false);
        host.Dispose();
    }
}
