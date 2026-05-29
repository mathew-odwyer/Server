using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation.Services.Sessions;
using Winterhaven.Gateway.Presentation.Services.Targets;

namespace Winterhaven.Gateway.Tests.Presentation.Services.Sessions;

[TestFixture]
internal sealed class RpcWebSocketSessionTests
{
    private ILogger<RpcWebSocketSession> logger;

    private ILoggerFactory loggerFactory;

    private RpcWebSocketSession session;

    private IJsonRpcTargetRegistrar targetRegistrar;

    [Test]
    public void ConstructorShouldThrowWhenLoggerFactoryIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(logger, null, targetRegistrar));

    [Test]
    public void ConstructorShouldThrowWhenLoggerIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(null, loggerFactory, targetRegistrar));

    [Test]
    public void ConstructorShouldThrowWhenTargetRegistrarIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(logger, loggerFactory, null));

    [TearDown]
    public void Dispose() => loggerFactory.Dispose();

    [Test]
    public async Task RunAsyncShouldRegisterTargetsOnStartup()
    {
        // Arrange
        var socket = Substitute.For<WebSocket>();
        socket.State.Returns(WebSocketState.Open);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        try
        {
            await session.RunAsync(socket, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Expected — cancellation propagates up intentionally to WebSocketMiddleware.
        }

        // Assert
        targetRegistrar.Received(1).RegisterTargets(Arg.Any<JsonRpc>());
        cts.Dispose();
    }

    [Test]
    public void RunAsyncShouldThrowWhenSocketIsNull() =>
        Assert.ThrowsAsync<ArgumentNullException>(() => session.RunAsync(null));

    [SetUp]
    public void SetUp()
    {
        logger = Substitute.For<ILogger<RpcWebSocketSession>>();
        loggerFactory = Substitute.For<ILoggerFactory>();
        targetRegistrar = Substitute.For<IJsonRpcTargetRegistrar>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        session = new RpcWebSocketSession(logger, loggerFactory, targetRegistrar);
    }
}
