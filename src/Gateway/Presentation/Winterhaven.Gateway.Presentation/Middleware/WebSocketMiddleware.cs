namespace Winterhaven.Gateway.Presentation.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Winterhaven.Gateway.Presentation;
using Winterhaven.Gateway.Presentation.Filters;
using Winterhaven.Gateway.Presentation.Targets.Services;

internal sealed class WebSocketMiddleware
{
    private readonly ILogger<WebSocketMiddleware> logger;

    private readonly RequestDelegate next;

    public WebSocketMiddleware(ILogger<WebSocketMiddleware> logger, RequestDelegate next)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, ILoggerFactory loggerFactory, JsonRpcRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(loggerFactory);

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

        var clientIp = context.Connection.RemoteIpAddress;
        string connectionId = context.Connection.Id;

        this.logger.LogInformation("Client connected. ConnectionId: {ConnectionId}, IP: {ClientIp}", connectionId, clientIp);

        var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

        using var formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            }
        };

        using var handler = new FilteringMessageHandler(loggerFactory.CreateLogger<FilteringMessageHandler>(), socket, formatter);
        using var rpc = new GatewayJsonRpc(loggerFactory.CreateLogger<GatewayJsonRpc>(), handler);

        registrar.RegisterTargets(rpc);

        rpc.StartListening();

        try
        {
            await rpc.Completion.WaitAsync(context.RequestAborted).ConfigureAwait(false);
            this.logger.LogInformation("Client disconnected gracefully. ConnectionId: {ConnectionId}, IP: {ClientIp}", connectionId, clientIp);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogInformation("Client disconnected abruptly. ConnectionId: {ConnectionId}, IP: {ClientIp}", connectionId, clientIp);
        }
    }
}