using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;

namespace Winterhaven.Gateway.Presentation.Handlers;

/// <summary>
///   Provides a message handler that masks outbound protocol-level JSON-RPC errors (negative codes) before they reach the client.
/// </summary>
/// <remarks>
///   Application errors from <see cref="GatewayJsonRpc"/> use positive codes (400, 401, -32603 internal) and are passed through unchanged.
/// </remarks>
internal sealed class GatewayWebSocketMessageHandler : WebSocketMessageHandler
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="GatewayWebSocketMessageHandler"/> class.
    /// </summary>
    /// <param name="webSocket">
    ///   The <see cref="WebSocket"/> used to communicate. This will <em>not</em> be automatically disposed of with this <see cref="WebSocketMessageHandler"/>.
    /// </param>
    /// <param name="formatter">
    ///   The formatter to use to serialize <see cref="JsonRpcMessage"/> instances.
    /// </param>
    /// <param name="sizeHint">
    ///   The size of the buffer to use for reading JSON-RPC messages. Messages which exceed this size will be handled properly but may require multiple I/O operations.
    /// </param>
    public GatewayWebSocketMessageHandler(WebSocket webSocket, IJsonRpcMessageFormatter formatter, int sizeHint = 4096)
        : base(webSocket, formatter, sizeHint)
    {
    }

    protected override ValueTask WriteCoreAsync(JsonRpcMessage content, CancellationToken cancellationToken)
    {
        if (content is JsonRpcError { Error: not null } error)
        {
            int code = (int)error.Error.Code;

            //// Negative codes are protocol/reserved errors (method not found, etc).
            //// Replace them so internal method names and routing details
            //// are never leaked to the client.
            if (code < 0)
            {
                error.Error = new JsonRpcError.ErrorDetail
                {
                    Code = JsonRpcErrorCode.InternalError,
                    Message = "An unexpected error occurred. Please try again later.",
                };
            }
        }

        return base.WriteCoreAsync(content, cancellationToken);
    }
}
