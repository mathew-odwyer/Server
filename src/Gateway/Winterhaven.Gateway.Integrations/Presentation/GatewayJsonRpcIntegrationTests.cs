using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Gateway.Integrations.Presentation.Targets;
using Winterhaven.Gateway.Presentation;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Integrations.Presentation;

[TestFixture]
internal sealed class GatewayJsonRpcIntegrationTests
{
    private IHost host;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Arrange
        var handler = Substitute.For<IJsonRpcMessageHandler>();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(null, handler));
    }

    [Test]
    public async Task CreateErrorDetailsShouldReturnAuthorizationErrorWhenAuthorizationExceptionIsThrown()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        byte[] request = Encoding.UTF8.GetBytes(/*lang=json,strict*/ "{\"jsonrpc\":\"2.0\",\"method\":\"test.throwAuthorization\",\"id\":2}");

        // Act
        await webSocket.SendAsync(request, WebSocketMessageType.Text, true, CancellationToken.None)
            .ConfigureAwait(false);

        byte[] buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
        string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Does.Contain("\"id\":2"));
            Assert.That(response, Does.Contain("\"error\""));
            Assert.That(response, Does.Contain("\"code\":401"));
            Assert.That(response, Does.Contain("Authorization failed."));
        }
    }

    [Test]
    public async Task CreateErrorDetailsShouldReturnInternalErrorWhenUnhandledExceptionIsThrown()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        byte[] request = Encoding.UTF8.GetBytes(/*lang=json,strict*/ "{\"jsonrpc\":\"2.0\",\"method\":\"test.throwUnhandled\",\"id\":3}");

        // Act
        await webSocket.SendAsync(request, WebSocketMessageType.Text, true, CancellationToken.None)
            .ConfigureAwait(false);

        byte[] buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
        string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Does.Contain("\"id\":3"));
            Assert.That(response, Does.Contain("\"error\""));
            Assert.That(response, Does.Contain("An unexpected error occurred. Please try again later."));
        }
    }

    [Test]
    public async Task CreateErrorDetailsShouldReturnValidationErrorWhenValidationExceptionIsThrown()
    {
        // Arrange
        var client = host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri("ws://localhost/ws"), CancellationToken.None)
            .ConfigureAwait(false);

        byte[] request = Encoding.UTF8.GetBytes(/*lang=json,strict*/ "{\"jsonrpc\":\"2.0\",\"method\":\"test.throwValidation\",\"id\":1}");

        // Act
        await webSocket.SendAsync(request, WebSocketMessageType.Text, true, CancellationToken.None)
            .ConfigureAwait(false);

        byte[] buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
        string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Does.Contain("\"id\":1"));
            Assert.That(response, Does.Contain("\"error\""));
            Assert.That(response, Does.Contain("\"code\":400"));
            Assert.That(response, Does.Contain("Validation failed."));
        }
    }

    [SetUp]
    public async Task Setup()
    {
        var builder = new HostBuilder();
        builder.ConfigureWebHost(x => x
             .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.Tests.json", optional: false))
             .ConfigureServices(x => x.AddScoped<IRpcTarget, ErrorThrowingTestTarget>())
             .UseTestServer()
             .UseStartup<Startup>());

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
