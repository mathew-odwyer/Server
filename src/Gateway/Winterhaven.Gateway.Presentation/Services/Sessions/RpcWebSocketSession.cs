using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation.Handlers;
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

        // Ensure formatting between C# <-> GML styling is considered.
        using var formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            }
        };

        using var handler = new GatewayWebSocketMessageHandler(loggerFactory.CreateLogger<GatewayWebSocketMessageHandler>(), socket, formatter);
        using var rpc = new GatewayJsonRpc(loggerFactory.CreateLogger<GatewayJsonRpc>(), handler);

        targetRegistrar.RegisterTargets(rpc);
        rpc.StartListening();

        // TODO: finally block: logout the user if they've logged in.

        try
        {
            // Finally, start the session, and only complete once the socket has disconnected or the user logs out.
            await rpc.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            //// Client sent malformed JSON - not a valid JSON-RPC message, keep the connection in case of older clients
            //// But also for testing purposes. This also ensures the server won't end up with a massive stacktrace
            //// and ensures it won't crash if someone attempts to connect and play the game with an older client.
            //// Also, this is handled in the GatewayWebSocketMessageHandler, but broken messages may still surface here
            //// and logging in debug because bad or old clients could make this noisy.
            logger.LogDebug("Client sent malformed JSON: {Message}", ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Source == "StreamJsonRpc")
        {
            //// Client sent structurally invalid JSON-RPC (e.g. wrong argument kind)
            //// This is good to log for debugging purposes but would be noisy in production.
            //// It's also good for the reasons mentioned in the catch above.
            logger.LogDebug("Client sent invalid JSON-RPC payload: {Message}", ex.Message);
        }
    }
}
