namespace Winterhaven.Gateway.Presentation.Filters;

using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Presentation.Extensions;

internal sealed class FilteringMessageHandler : WebSocketMessageHandler
{
    private readonly ILogger<FilteringMessageHandler> logger;

    public FilteringMessageHandler(ILogger<FilteringMessageHandler> logger, WebSocket socket, IJsonRpcMessageFormatter formatter)
        : base(socket, formatter)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async ValueTask<JsonRpcMessage?> ReadCoreAsync(CancellationToken cancellationToken)
    {
        JsonRpcMessage? message;

        try
        {
            message = await base.ReadCoreAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException)
        {
            this.logger.LogDebug("Invalid JSON received from client. ConnectionId: {ConnectionId}", this.WebSocket.GetHashCode());
            await this.WebSocket.SafeCloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid JSON Payload", cancellationToken).ConfigureAwait(false);
            return null;
        }
        catch (WebSocketException)
        {
            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        if (message is JsonRpcError error)
        {
            switch (error.Error?.Code ?? JsonRpcErrorCode.InvalidRequest)
            {
                case JsonRpcErrorCode.InvalidRequest:
                    await this.WebSocket.SafeCloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid Request", cancellationToken).ConfigureAwait(false);
                    return null;

                case JsonRpcErrorCode.ParseError:
                    await this.WebSocket.SafeCloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Parse Error", cancellationToken).ConfigureAwait(false);
                    return null;
            }
        }

        return message;
    }

    protected override ValueTask WriteCoreAsync(JsonRpcMessage content, CancellationToken cancellationToken)
    {
        if (content is JsonRpcError error)
        {
            int code = (int)(error.Error?.Code ?? JsonRpcErrorCode.InvalidRequest);

            // any negative code is a protocol/reserved error, mask it.
            if (code < 0)
            {
                this.logger.LogDebug("RPC protocol violation. Code: {Code}, Message: {Message}", code, error.Error?.Message);

                error.Error = new JsonRpcError.ErrorDetail()
                {
                    Code = JsonRpcErrorCode.InternalError,
                    Message = "An unexpected error occurred. Please try again later.",
                };
            }
        }

        return base.WriteCoreAsync(content, cancellationToken);
    }
}