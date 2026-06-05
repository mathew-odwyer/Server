using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using Winterhaven.Gateway.Integrations.Services.Users;

namespace Winterhaven.Gateway.Integrations.Presentation.Services.Sessions;

[TestFixture]
internal sealed class WebSocketRpcSessionIntegrationTests : TestHostBase
{
    [Test]
    public async Task RunAsyncShouldDisconnectClientWhenSessionHasInvalidated()
    {
        // Arrange
        await using var connection = await CreateConnectionAsync(null);

        UserSessionManager.EstablishUserSession(MockUserSessionManager.DummySession);

        // Act
        UserSessionManager.InvalidateUserSession();

        // TODO: This needs to be addressed, Task.Delay is a hack solution.
        await Task.Delay(500);

        // Assert
        Assert.That(connection.State, Is.EqualTo(WebSocketState.Closed));
    }

    [Test]
    public async Task RunAsyncShouldRespondToValidJsonRpcRequest()
    {
        // Arrange
        var client = Host.GetTestServer().CreateWebSocketClient();
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
        var client = Host.GetTestServer().CreateWebSocketClient();
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
        var client = Host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        byte[] request = Encoding.UTF8.GetBytes(/*lang=json,strict*/ """
        {
            "jsonrpc": "2.0",
            "method": "unknown.method",
            "id": 1
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
        Assert.That(response, Does.Contain("\"error\""));
    }
}
