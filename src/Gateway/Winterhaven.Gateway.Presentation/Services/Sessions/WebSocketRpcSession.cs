using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Handlers;
using Winterhaven.Gateway.Presentation.Services.Events;
using Winterhaven.Gateway.Presentation.Services.Targets;

namespace Winterhaven.Gateway.Presentation.Services.Sessions;

internal sealed class WebSocketRpcSession : IWebSocketRpcSession
{
    private readonly IEventForwarderCoordinator eventForwarderCoordinator;

    private readonly ILogger<WebSocketRpcSession> logger;

    private readonly ILoggerFactory loggerFactory;

    private readonly IJsonRpcTargetRegistrar targetRegistrar;

    private readonly IUserAccountService userAccountService;

    private readonly IUserSessionContext userSessionContext;

    public WebSocketRpcSession(
        ILogger<WebSocketRpcSession> logger,
        ILoggerFactory loggerFactory,
        IJsonRpcTargetRegistrar targetRegistrar,
        IUserAccountService userAccountService,
        IUserSessionContext userSessionContext,
        IEventForwarderCoordinator eventForwarderCoordinator)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        this.targetRegistrar = targetRegistrar ?? throw new ArgumentNullException(nameof(targetRegistrar));
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
        this.eventForwarderCoordinator = eventForwarderCoordinator ?? throw new ArgumentNullException(nameof(eventForwarderCoordinator));
    }

    public async Task RunAsync(WebSocket socket, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(socket);

        // Ensure formatting between C# <-> GML styling is considered.
        using var formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                ////UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            }
        };

        using var handler = new GatewayWebSocketMessageHandler(this.loggerFactory.CreateLogger<GatewayWebSocketMessageHandler>(), socket, formatter);
        using var rpc = new GatewayJsonRpc(this.loggerFactory.CreateLogger<GatewayJsonRpc>(), this.userSessionContext, handler);

        // Ensure that the connection will be closed if the user session is invalidated.
        void OnSessionInvalidated(object? sender, EventArgs e)
        {
            if (!rpc.IsDisposed)
            {
                rpc.Dispose();
            }
        }

        void OnSessionEstablished(object? sender, EventArgs e)
        {
            if (rpc.IsDisposed)
            {
                return;
            }

            //// Fire and forget, we're registering Services -> NATS -> Gateway -> Client forwarders.
            //// No manual call to DisposeAsync is required thanks to services being scoped.
            this.eventForwarderCoordinator.StartAllForwardersAsync(rpc, cancellationToken);
        }

        this.userSessionContext.Invalidated += OnSessionInvalidated;
        this.userSessionContext.Established += OnSessionEstablished;

        try
        {
            this.targetRegistrar.RegisterTargets(rpc);
            rpc.StartListening();

            // Finally, start the session, and only complete once the socket has disconnected.
            await rpc.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            //// Client sent malformed JSON - not a valid JSON-RPC message, keep the connection in case of older clients
            //// But also for testing purposes. This also ensures the server won't end up with a massive stacktrace
            //// and ensures it won't crash if someone attempts to connect and play the game with an older client.
            //// Also, this is handled in the GatewayWebSocketMessageHandler, but broken messages may still surface here
            //// and logging in debug because bad or old clients could make this noisy.
            this.logger.LogDebug(ex, "Client sent malformed JSON.");
        }
        catch (InvalidOperationException ex) when (ex.Source == "StreamJsonRpc")
        {
            //// Client sent structurally invalid JSON-RPC (e.g. wrong argument kind)
            //// This is good to log for debugging purposes but would be noisy in production.
            //// It's also good for the reasons mentioned in the catch above.
            this.logger.LogDebug(ex, "Client sent invalid JSON-RPC payload.");
        }
        finally
        {
            this.userSessionContext.Established -= OnSessionEstablished;
            this.userSessionContext.Invalidated -= OnSessionInvalidated;

            try
            {
                await this.userAccountService.LogoutAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (AuthorizationException ex)
            {
                //// A 401 here is expected when the session was already invalidated server-side,
                //// For exampale, after a failed token refresh.
                if (this.userSessionContext.UserSession != null)
                {
                    this.logger.LogWarning(ex, "Logout request for user with ID '{Username}' was rejected (session likely not refreshed).", this.userSessionContext.UserSession.UserAccountId);
                }
            }
        }
    }
}
