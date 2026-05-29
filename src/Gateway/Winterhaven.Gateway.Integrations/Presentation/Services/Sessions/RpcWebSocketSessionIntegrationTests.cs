using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Winterhaven.Gateway.Presentation;

namespace Winterhaven.Gateway.Integrations.Presentation.Services.Sessions;

[TestFixture]
internal sealed class RpcWebSocketSessionIntegrationTests
{
    private IHost host;

    [Test]
    public async Task RunAsyncShouldRespondToValidJsonRpcRequest()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        byte[] request = Encoding.UTF8.GetBytes(/*lang=json,strict*/ """
        {
            "jsonrpc": "2.0",
            "method": "health.ping",
            "id": 1,
            "params": {
            "time_stamp": 0
            }
        }
        """);

        // Act
        await webSocket.SendAsync(request, WebSocketMessageType.Text, true, CancellationToken.None)
            .ConfigureAwait(false);

        byte[] buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
        string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Assert
        Assert.That(response, Does.Contain("\"id\":1"));
        Assert.That(response, Does.Not.Contain("\"error\""));
    }

    [Test]
    public async Task RunAsyncShouldReturnErrorForMalformedJson()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        byte[] malformedRequest = Encoding.UTF8.GetBytes("this is not json");

        // Act
        await webSocket.SendAsync(malformedRequest, WebSocketMessageType.Text, true, CancellationToken.None)
            .ConfigureAwait(false);

        byte[] buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);

        // Assert
        Assert.That(result.MessageType, Is.EqualTo(WebSocketMessageType.Close));
    }

    [Test]
    public async Task RunAsyncShouldReturnErrorForUnknownMethod()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

#pragma warning disable JSON002 // Probable JSON string detected
        byte[] request = Encoding.UTF8.GetBytes("""{"jsonrpc":"2.0","method":"unknown/method","id":2}""");
#pragma warning restore JSON002 // Probable JSON string detected

        // Act
        await webSocket.SendAsync(request, WebSocketMessageType.Text, true, CancellationToken.None)
            .ConfigureAwait(false);

        byte[] buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
        string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Assert
        Assert.That(response, Does.Contain("\"id\":2"));
        Assert.That(response, Does.Contain("\"error\""));
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
