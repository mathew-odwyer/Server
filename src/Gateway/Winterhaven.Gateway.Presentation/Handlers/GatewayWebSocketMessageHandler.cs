using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<GatewayWebSocketMessageHandler> logger;

    /// <summary>
    ///   Initializes a new instance of the <see cref="GatewayWebSocketMessageHandler"/> class.
    /// </summary>
    /// <param name="logger">
    ///   The logger.
    /// </param>
    /// <param name="webSocket">
    ///   The <see cref="WebSocket"/> used to communicate. This will <em>not</em> be automatically disposed of with this <see cref="WebSocketMessageHandler"/>.
    /// </param>
    /// <param name="formatter">
    ///   The formatter to use to serialize <see cref="JsonRpcMessage"/> instances.
    /// </param>
    /// <param name="sizeHint">
    ///   The size of the buffer to use for reading JSON-RPC messages. Messages which exceed this size will be handled properly but may require multiple I/O operations.
    /// </param>
    public GatewayWebSocketMessageHandler(
        ILogger<GatewayWebSocketMessageHandler> logger,
        WebSocket webSocket,
        IJsonRpcMessageFormatter formatter,
        int sizeHint = 4096)
        : base(webSocket, formatter, sizeHint) =>
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///   Serializes and writes a <see cref="JsonRpcMessage"/> to the WebSocket, masking protocol-level errors before transmission.
    /// </summary>
    /// <param name="content">
    ///   The <see cref="JsonRpcMessage"/> to write.
    /// </param>
    /// <param name="cancellationToken">
    ///   A <see cref="CancellationToken"/> that can be used to abort the write operation.
    /// </param>
    /// <returns>
    ///   A <see cref="ValueTask"/> that completes when the message has been written to the underlying WebSocket.
    /// </returns>
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
                switch ((JsonRpcErrorCode)code)
                {
                    case JsonRpcErrorCode.MethodNotFound:
                        //// Expected in production, so just log as debug.
                        //// This is because clients can make their own requests.
                        logger.LogDebug("Client requested unknown method. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.InvalidParams:
                        // Expected in production, just log as debug.
                        logger.LogDebug("Client sent invalid params. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.ParseError:
                    case JsonRpcErrorCode.InvalidRequest:
                        // Expected in production, just log as debug.
                        logger.LogDebug("Client sent malformed request. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.InternalError:
                        // Log an error, this is a server side bug most likely.
                        logger.LogError("Internal JSON-RPC error occurred. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.InvocationError:
                        // The target method threw an exception, server-side fault.
                        logger.LogWarning("Method invocation failed. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.InvocationErrorWithException:
                        // Same as above but with a serialized exception attached, server-side fault. Arguably more severe since it carries exception detail; keep at Warning.
                        logger.LogWarning("Method invocation failed with exception detail. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.ResponseSerializationFailure:
                        // Your server produced a response it then couldn't serialize, server-side fault. This one is worth Error since it indicates a broken contract on your side.
                        logger.LogError("Failed to serialize RPC response. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.NoMarshaledObjectFound:
                        // Client referenced a marshaled object handle that no longer exists. Could be a race (Debug) or a bad actor replaying stale handles.
                        logger.LogDebug("Client referenced an unknown or expired marshaled object. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    case JsonRpcErrorCode.RequestCanceled:
                        // Client cancelled the request, completely normal, not an error at all.
                        logger.LogDebug("Client cancelled a pending request. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;

                    default:
                        logger.LogDebug("Unrecognised protocol error. Code: {Code}, Message: {Message}", code, error.Error.Message);
                        break;
                }

                // Lastly, make sure we return a generic error to the client.
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
