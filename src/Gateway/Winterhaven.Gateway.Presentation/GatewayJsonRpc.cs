using System;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;

namespace Winterhaven.Gateway.Presentation;

internal sealed class GatewayJsonRpc : JsonRpc
{
    private readonly ILogger<GatewayJsonRpc> logger;

    public GatewayJsonRpc(
        ILogger<GatewayJsonRpc> logger,
        IJsonRpcMessageHandler messageHandler)
        : base(messageHandler) => this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
    {
        logger.LogTrace("Creating error details for exception: '{Name}'", exception.GetType().Name);

        switch (exception)
        {
            //// TODO: ValidationException
            //// TODO: AuthorizationException

            // Everything else: log it server-side, send nothing useful to the client.
            default:
                logger.LogError(exception, "Unhandled exception processing RPC method '{Method}'.", request.Method);

                return new JsonRpcError.ErrorDetail()
                {
                    Code = JsonRpcErrorCode.InternalError,
                    Message = "An unexpected error occurred. Please try again later.",
                };
        }
    }
}
