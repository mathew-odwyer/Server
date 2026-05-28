using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Presentation.Middleware;

namespace Winterhaven.Gateway.Tests.Presentation.Middleware;

[TestFixture]
internal sealed class WebSocketMiddlewareTests
{
    private ILogger<WebSocketMiddleware> logger;

    private WebSocketMiddleware middleware;

    private RequestDelegate requestDelegate;

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
        await middleware.InvokeAsync(httpContext).ConfigureAwait(false);

        // Assert
        Assert.That(isCalled, Is.True);
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
        await middleware.InvokeAsync(httpContext).ConfigureAwait(false);

        // Assert
        Assert.That(isCalled, Is.False);
    }

    [Test]
    public async Task InvokeAsyncShouldRespondWithBadRequestWhenNotWebSocketRequest()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/ws";

        // Act
        await middleware.InvokeAsync(httpContext).ConfigureAwait(false);

        // Assert
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public void InvokeAsyncShouldThrowArgumentNullExceptionWhenContextIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => middleware.InvokeAsync(null));

    [SetUp]
    public void SetUp()
    {
        webSocket = new ClientWebSocket();
        logger = Substitute.For<ILogger<WebSocketMiddleware>>();
        requestDelegate = new RequestDelegate(_ => Task.CompletedTask);
        middleware = new WebSocketMiddleware(logger, requestDelegate);
    }

    [TearDown]
    public void TearDown() => webSocket.Dispose();
}
