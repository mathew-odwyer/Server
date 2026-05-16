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
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Extensions;
using Winterhaven.Gateway.Presentation.Filters;

internal sealed class WebSocketRpcSession
{
    private readonly ILogger<WebSocketRpcSession> logger;

    private readonly ILoggerFactory loggerFactory;

    private readonly JsonRpcRegistrar registrar;

    private readonly IUserAccountService userAccountService;

    private readonly ISessionAuthenticator sessionAuthenticator;

    public WebSocketRpcSession(
        ILogger<WebSocketRpcSession> logger,
        ILoggerFactory loggerFactory,
        JsonRpcRegistrar registrar,
        IUserAccountService userAccountService,
        ISessionAuthenticator sessionAuthenticator)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        this.registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
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

        using var handler = new FilteringMessageHandler(this.loggerFactory.CreateLogger<FilteringMessageHandler>(), socket, formatter);
        using var rpc = new GatewayJsonRpc(this.loggerFactory.CreateLogger<GatewayJsonRpc>(), handler, this.sessionAuthenticator);

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
            await this.userAccountService.LogoutUserAsync(CancellationToken.None).ConfigureAwait(false);

            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await socket.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Session Ended", cancellationToken).ConfigureAwait(false);
            }

            socket.Dispose();
        }
    }
}