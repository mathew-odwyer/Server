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

        //// Only return errors that are useful to the client
        //// We also chose to use positive codes that can (sometimes) match HTTP Status Codes.
        //// This isn't a requirement, but just a nice-to-have.
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

            //// For everything else, log it and send nothing useful to the client.
            //// It's obviously a bug or some other issue - let's not leak any internals to the client.
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
