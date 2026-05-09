namespace Winterhaven.Gateway.Presentation.Middleware;

using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Winterhaven.Gateway.Presentation.Targets;

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

        // If the request is not through /ws endpoint, continue to the next pipeline.
        if (context.Request.Path != "/ws")
        {
            await this.next(context).ConfigureAwait(false);
            return;
        }

        // If it is a /ws endpoint, we should return a 400 back to the client.
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using (var scope = context.RequestServices.CreateAsyncScope())
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        {
            using (var formatter = new JsonMessageFormatter())
            {
                formatter.JsonSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();

                using (var handler = new WebSocketMessageHandler(socket, formatter))
                {
                    using (var rpc = new GatewayJsonRpc(scope.ServiceProvider.GetRequiredService<ILogger<GatewayJsonRpc>>(), handler))
                    {
                        foreach (var target in scope.ServiceProvider.GetServices<RpcTargetBase>())
                        {
                            rpc.AddLocalRpcTarget(target);
                        }

                        rpc.StartListening();

                        try
                        {
                            await rpc.Completion.ConfigureAwait(false);
                        }
                        catch (WebSocketException ex) when (ex.InnerException is ConnectionResetException)
                        {
                            this.logger.LogDebug("Client disconnected abruptly (no close handshake).");
                        }
                    }
                }
            }
        }
    }
}