using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation.Services.Targets;

namespace Winterhaven.Gateway.Presentation.Services.Sessions;

internal sealed class RpcWebSocketSession : IRpcWebSocketSession
{
    private readonly ILogger<RpcWebSocketSession> logger;

    private readonly ILoggerFactory loggerFactory;

    private readonly IJsonRpcTargetRegistrar targetRegistrar;

    public RpcWebSocketSession(
        ILogger<RpcWebSocketSession> logger,
        ILoggerFactory loggerFactory,
        IJsonRpcTargetRegistrar targetRegistrar)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        this.targetRegistrar = targetRegistrar ?? throw new ArgumentNullException(nameof(targetRegistrar));
    }

    public async Task RunAsync(WebSocket socket, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(socket);

        using var formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            }
        };

        using var handler = new WebSocketMessageHandler(socket, formatter);
        using var rpc = new GatewayJsonRpc(loggerFactory.CreateLogger<GatewayJsonRpc>(), handler);

        targetRegistrar.RegisterTargets(rpc);
        rpc.StartListening();

        // TODO: finally block: logout the user if they've logged in.

        try
        {
            await rpc.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            // Client sent malformed JSON - not a valid JSON-RPC message at all.
            logger.LogWarning("Client sent malformed JSON: {Message}", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // Client sent structurally invalid JSON-RPC (e.g. wrong argument kind).
            logger.LogWarning("Client sent invalid JSON-RPC payload: {Message}", ex.Message);
        }
    }
}
