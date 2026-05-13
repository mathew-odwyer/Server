namespace Winterhaven.Gateway.Presentation.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Presentation.Extensions;
using Winterhaven.Gateway.Presentation.Filters;

internal sealed class WebSocketRpcSession
{
    private readonly ILogger<WebSocketRpcSession> logger;

    private readonly ILoggerFactory loggerFactory;

    private readonly JsonRpcRegistrar registrar;

    public WebSocketRpcSession(ILogger<WebSocketRpcSession> logger, ILoggerFactory loggerFactory, JsonRpcRegistrar registrar)
    {
        this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        this.loggerFactory = loggerFactory ?? throw new System.ArgumentNullException(nameof(loggerFactory));
        this.registrar = registrar ?? throw new System.ArgumentNullException(nameof(registrar));
    }

    public async Task RunAsync(HttpContext context, WebSocket socket, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(socket);

        var clientIp = context.Connection.RemoteIpAddress;
        string clientId = context.Connection.Id;

        this.logger.LogInformation("Starting RPC session. ConnectionId: {ConnectionId}, IP: {ClientIp}", clientId, clientIp);

        using var formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            }
        };

        using var handler = new FilteringMessageHandler(loggerFactory.CreateLogger<FilteringMessageHandler>(), socket, formatter);
        using var rpc = new GatewayJsonRpc(this.loggerFactory.CreateLogger<GatewayJsonRpc>(), handler);

        this.registrar.RegisterTargets(rpc);

        rpc.StartListening();

        try
        {
            await rpc.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
            this.logger.LogInformation("Client disconnected gracefully. ConnectionId: {ConnectionId}, IP: {ClientIp}", clientId, clientIp);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogInformation("Client disconnected abruptly. ConnectionId: {ConnectionId}, IP: {ClientIp}", clientId, clientIp);
        }
        finally
        {
            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await socket.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Session Ended", cancellationToken).ConfigureAwait(false);
            }

            socket.Dispose();
        }
    }
}
