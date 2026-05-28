using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

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

        logger.LogInformation("WebSocket Client Connected: Client ID: '{ClientId}', Client IP: '{ClientIp}'", clientId, clientIp);
    }
}
