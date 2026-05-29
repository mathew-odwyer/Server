using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using Winterhaven.Gateway.Core.Domain.Exceptions;

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
            case ValidationException validationException:
                return new JsonRpcError.ErrorDetail
                {
                    Code = (JsonRpcErrorCode)400,
                    Message = validationException.Message,
                    Data = validationException.Errors ?? new Dictionary<string, string[]>
                    {
                        { "General", ["One or more validation errors occurred."] }
                    },
                };

            case AuthorizationException authorizationException:
                return new JsonRpcError.ErrorDetail
                {
                    Code = (JsonRpcErrorCode)401,
                    Message = authorizationException.Message,
                };

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
