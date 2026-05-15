namespace Winterhaven.Gateway.Presentation.Middleware;

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Winterhaven.Gateway.Presentation.Services;

internal sealed class WebSocketMiddleware
{
    private readonly RequestDelegate next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, WebSocketRpcSession session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        if (context.Request.Path != "/ws")
        {
            await this.next(context).ConfigureAwait(false);
            return;
        }

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
        await session.RunAsync(context, socket, context.RequestAborted).ConfigureAwait(false);
    }
}
