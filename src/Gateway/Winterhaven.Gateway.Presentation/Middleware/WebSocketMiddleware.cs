using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Winterhaven.Gateway.Presentation.Extensions;
using Winterhaven.Gateway.Presentation.Services.Sessions;

namespace Winterhaven.Gateway.Presentation.Middleware;

internal sealed class WebSocketMiddleware
{
    private readonly ILogger<WebSocketMiddleware> logger;

    private readonly RequestDelegate next;

    public WebSocketMiddleware(ILogger<WebSocketMiddleware> logger, RequestDelegate next)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, IRpcWebSocketSession session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        // Only accept if the routing is correct.
        if (context.Request.Path != "/ws")
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        // Return a bad request if the request is not a websocket request.
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

        var clientIp = context.Connection.RemoteIpAddress;
        string clientId = context.Connection.Id;

        logger.LogDebug("WebSocket Client Connected: Client ID: '{ClientId}', Client IP: '{ClientIp}'", clientId, clientIp);

        try
        {
            //// Run the RPC session, abort if the client disconnects.
            //// Whether any exception is caught or not, we still properly dispose of the connection in the finally block.
            logger.LogInformation("Starting RPC session: Client ID: '{ClientId}', Client IP: '{ClientIp}'", clientId, clientIp);
            await session.RunAsync(socket, context.RequestAborted).ConfigureAwait(false);
            logger.LogInformation("WebSocket Client Disconnected: Client ID: '{ClientId}', Client IP: '{ClientIp}'", clientId, clientIp);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            //// Client disconnected abruptly (closed the window, network dropped, etc).
            logger.LogInformation("WebSocket Client Disconnected Abruptly. ConnectionId: {ConnectionId}, IP: {ClientIp}", clientId, clientIp);
        }
        finally
        {
            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await socket.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Session Ended", CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}
