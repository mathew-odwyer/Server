using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
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

    [Test]
    public async Task InvokeAsyncShouldPassThroughWhenPathIsNotWs()
    {
        var client = host.GetTestServer().CreateClient();

        var response = await client.GetAsync(new Uri("http://localhost/health")).ConfigureAwait(false);

        Assert.That((int)response.StatusCode, Is.Not.EqualTo(400));
    }

    [Test]
    public async Task InvokeAsyncShouldReturn400WhenRequestIsNotWebSocket()
    {
        var client = host.GetTestServer().CreateClient();

        var response = await client.GetAsync(new Uri("http://localhost/ws")).ConfigureAwait(false);

        Assert.That((int)response.StatusCode, Is.EqualTo(400));
    }

    [SetUp]
    public async Task Setup()
    {
        var builder = new HostBuilder();

        builder.ConfigureWebHost(x =>
        {
            x.ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.Tests.json", optional: false));
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
