using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Presentation.Middleware;
using Winterhaven.Gateway.Presentation.Services.Sessions;

namespace Winterhaven.Gateway.Tests.Presentation.Middleware;

[TestFixture]
internal sealed class WebSocketMiddlewareTests
{
    private ILogger<WebSocketMiddleware> logger;

    private WebSocketMiddleware middleware;

    private RequestDelegate requestDelegate;

    private IWebSocketRpcSession rpcSession;

    private ClientWebSocket webSocket;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new WebSocketMiddleware(null, requestDelegate));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenNextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new WebSocketMiddleware(logger, null));

    [Test]
    public async Task InvokeAsyncShouldCloseSocketWhenSessionCompletesNormally()
    {
        var (context, socket) = BuildWebSocketHttpContext();
        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await middleware.InvokeAsync(context, rpcSession).ConfigureAwait(false);

        await socket.Received(1)
            .CloseAsync(WebSocketCloseStatus.NormalClosure, "Session Ended", CancellationToken.None)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task InvokeAsyncShouldCloseSocketWhenSocketStateIsCloseReceived()
    {
        // Arrange
        var (context, socket) = BuildWebSocketHttpContext(socketState: WebSocketState.CloseReceived);

        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, rpcSession).ConfigureAwait(false);

        // Assert
        await socket.Received(1)
            .CloseAsync(WebSocketCloseStatus.NormalClosure, "Session Ended", CancellationToken.None)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task InvokeAsyncShouldInvokeNextWhenRequestPathIsNotWs()
    {
        // Arrange
        bool isCalled = false;
        var httpContext = new DefaultHttpContext();

        var middleware = new WebSocketMiddleware(
            logger: logger,
            next: _ =>
            {
                isCalled = true;
                return Task.CompletedTask;
            });

        // Act
        await middleware.InvokeAsync(httpContext, rpcSession).ConfigureAwait(false);

        // Assert
        Assert.That(isCalled, Is.True);
    }

    [Test]
    public async Task InvokeAsyncShouldNotCloseSocketWhenSocketStateIsNotOpenOrCloseReceived()
    {
        // Arrange
        var (context, socket) = BuildWebSocketHttpContext(socketState: WebSocketState.Aborted);

        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, rpcSession).ConfigureAwait(false);

        // Assert
        await socket.DidNotReceive()
            .CloseAsync(Arg.Any<WebSocketCloseStatus>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task InvokeAsyncShouldNotInvokeNextWhenRequestPathIsWs()
    {
        // Arrange
        bool isCalled = false;
        var httpContext = new DefaultHttpContext();

        httpContext.Request.Path = "/ws";

        var middleware = new WebSocketMiddleware(
            logger: logger,
            next: _ =>
            {
                isCalled = true;
                return Task.CompletedTask;
            });

        // Act
        await middleware.InvokeAsync(httpContext, rpcSession).ConfigureAwait(false);

        // Assert
        Assert.That(isCalled, Is.False);
    }

    [Test]
    public async Task InvokeAsyncShouldNotRethrowWhenRequestIsCancelled()
    {
        var cts = new CancellationTokenSource();
        var (context, _) = BuildWebSocketHttpContext(requestAborted: cts.Token);

        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new OperationCanceledException(cts.Token));

        await cts.CancelAsync().ConfigureAwait(false);

        Assert.DoesNotThrowAsync(() => middleware.InvokeAsync(context, rpcSession));
        cts.Dispose();
    }

    [Test]
    public async Task InvokeAsyncShouldPassAcceptedWebSocketToRunAsync()
    {
        // Arrange
        var (context, socket) = BuildWebSocketHttpContext();
        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, rpcSession).ConfigureAwait(false);

        // Assert
        await rpcSession.Received(1)
            .RunAsync(socket, Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task InvokeAsyncShouldPassRequestAbortedTokenToRunAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var (context, _) = BuildWebSocketHttpContext(requestAborted: cts.Token);

        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, rpcSession).ConfigureAwait(false);

        // Assert
        await rpcSession.Received(1)
            .RunAsync(Arg.Any<WebSocket>(), cts.Token)
            .ConfigureAwait(false);
    }

    [Test]
    public async Task InvokeAsyncShouldRespondWithBadRequestWhenNotWebSocketRequest()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/ws";

        // Act
        await middleware.InvokeAsync(httpContext, rpcSession).ConfigureAwait(false);

        // Assert
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public void InvokeAsyncShouldRethrowOperationCanceledExceptionWhenRequestIsNotCancelled()
    {
        // Arrange

        //// RequestAborted is default (not cancelled), so the `when` filter will be false.
        var (context, _) = BuildWebSocketHttpContext();

        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new OperationCanceledException());

        // Act and assert
        Assert.ThrowsAsync<OperationCanceledException>(() => middleware.InvokeAsync(context, rpcSession));
    }

    [Test]
    public void InvokeAsyncShouldRethrowWhenSessionThrowsUnexpectedException()
    {
        var (context, _) = BuildWebSocketHttpContext();
        rpcSession.RunAsync(Arg.Any<WebSocket>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new InvalidOperationException("Unexpected"));

        Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context, rpcSession));
    }

    [Test]
    public void InvokeAsyncShouldThrowArgumentNullExceptionWhenContextIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => middleware.InvokeAsync(null, rpcSession));

    [Test]
    public void InvokeAsyncShouldThrowArgumentNullExceptionWhenSessionIsNUll() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => middleware.InvokeAsync(new DefaultHttpContext(), null));

    [SetUp]
    public void SetUp()
    {
        webSocket = new ClientWebSocket();
        logger = Substitute.For<ILogger<WebSocketMiddleware>>();
        requestDelegate = new RequestDelegate(_ => Task.CompletedTask);
        rpcSession = Substitute.For<IWebSocketRpcSession>();
        middleware = new WebSocketMiddleware(logger, requestDelegate);
    }

    [TearDown]
    public void TearDown() => webSocket.Dispose();

    private static (HttpContext context, WebSocket socket) BuildWebSocketHttpContext(
        WebSocketState socketState = WebSocketState.Open,
        CancellationToken requestAborted = default)
    {
        var socket = Substitute.For<WebSocket>();
        socket.State.Returns(socketState);

        var webSocketManager = Substitute.For<WebSocketManager>();
        webSocketManager.IsWebSocketRequest.Returns(true);
        webSocketManager.AcceptWebSocketAsync().Returns(socket);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Path.Returns(new PathString("/ws"));
        httpContext.WebSockets.Returns(webSocketManager);
        httpContext.RequestAborted.Returns(requestAborted);
        httpContext.Connection.Returns(Substitute.For<ConnectionInfo>());

        return (httpContext, socket);
    }
}
