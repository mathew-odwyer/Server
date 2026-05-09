namespace Winterhaven.Gateway.Presentation.Targets;

using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;

internal sealed class GatewayJsonRpc : JsonRpc
{
    private readonly ILogger<GatewayJsonRpc> logger;

    public GatewayJsonRpc(ILogger<GatewayJsonRpc> logger, IJsonRpcMessageHandler messageHandler)
        : base(messageHandler)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.AddLocalRpcMethod("*", (string methodName) =>
        {
            logger.LogWarning("Client requested unknown RPC method '{Method}'.", methodName);
            throw new LocalRpcException("An unexpected error occurred. Please try again later.");
        });
    }

    protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
    {
        // Intentional game errors — let these through as-is, they're designed for the client.
        if (exception is LocalRpcException localRpcException)
        {
            return new JsonRpcError.ErrorDetail
            {
                Code = (JsonRpcErrorCode)localRpcException.ErrorCode,
                Message = localRpcException.Message,
                Data = localRpcException.ErrorData,
            };
        }

        // TODO: HttpExceptions, etc.

        // Everything else: log it server-side, send nothing useful to the client.
        this.logger.LogError(exception, "Unhandled exception processing RPC method '{Method}'.", request.Method);

        return new JsonRpcError.ErrorDetail()
        {
            Code = JsonRpcErrorCode.InternalError,
            Message = "An unexpected error occurred. Please try again later.",
        };
    }
}