namespace Winterhaven.Gateway.Presentation;

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
    }

    protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
    {
        switch (exception)
        {
            case InvalidOperationException:
                return new JsonRpcError.ErrorDetail()
                {
                    Code = (JsonRpcErrorCode)1,
                    Message = "Lol you couldn't login.",
                };

            default:
                // Everything else: log it server-side, send nothing useful to the client.
                this.logger.LogError(exception, "Unhandled exception processing RPC method '{Method}'.", request.Method);
                return new JsonRpcError.ErrorDetail()
                {
                    Code = JsonRpcErrorCode.InternalError,
                    Message = "An unexpected error occurred. Please try again later.",
                };
        }
    }
}