using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;
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

    private IUserAccountService userAccountService;

    ////[Test]
    ////public void ConstructorShouldThrowWhenLoggerFactoryIsNull() =>
    ////    Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(logger, null, targetRegistrar, userAccountService));

    ////[Test]
    ////public void ConstructorShouldThrowWhenLoggerIsNull() =>
    ////    Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(null, loggerFactory, targetRegistrar, userAccountService));

    ////[Test]
    ////public void ConstructorShouldThrowWhenTargetRegistrarIsNull() =>
    ////    Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(logger, loggerFactory, null, userAccountService));

    ////[Test]
    ////public void ConstructorShouldThrowWhenUserAccountServiceIsNull() =>
    ////    Assert.Throws<ArgumentNullException>(() => new RpcWebSocketSession(logger, loggerFactory, targetRegistrar, null));

    private IUserSessionAuthenticator userSessionAuthenticator;

    private IUserSessionContext userSessionContext;

    private IUserSessionExpiryNotifier userSessionExpiryNotifier;

    [TearDown]
    public void Dispose()
    {
        loggerFactory.Dispose();
        userSessionExpiryNotifier.Dispose();
    }

    [Test]
    public async Task RunAsyncShouldInvokeUserAccountServiceLogoutAsyncWhenSessionIsComplete()
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
            // Expected because cancellation propagates up intentionally to WebSocketMiddleware. This is because we let the exception through for the websocket middleware to handle it, as it's a protocol concern.
        }

        // Assert
        await userAccountService.Received(1).LogoutAsync(Arg.Any<CancellationToken>()).ConfigureAwait(false);
        cts.Dispose();
    }

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
            // Expected because cancellation propagates up intentionally to WebSocketMiddleware. This is because we let the exception through for the websocket middleware to handle it, as it's a protocol concern.
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
        userAccountService = Substitute.For<IUserAccountService>();
        userSessionContext = Substitute.For<IUserSessionContext>();
        userSessionExpiryNotifier = Substitute.For<IUserSessionExpiryNotifier>();
        userSessionAuthenticator = Substitute.For<IUserSessionAuthenticator>();

        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

        session = new RpcWebSocketSession(
            logger,
            loggerFactory,
            targetRegistrar,
            userAccountService,
            userSessionContext,
            userSessionExpiryNotifier,
            userSessionAuthenticator);
    }
}
