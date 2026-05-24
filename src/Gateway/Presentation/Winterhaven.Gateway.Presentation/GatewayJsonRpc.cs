namespace Winterhaven.Gateway.Presentation;

using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Presentation.Attributes;
using ValidationException = Core.Domain.Exceptions.ValidationException;

internal sealed class GatewayJsonRpc : JsonRpc
{
    private readonly ILogger<GatewayJsonRpc> logger;

    private readonly ISessionAuthenticator sessionAuthenticator;

    public GatewayJsonRpc(
        ILogger<GatewayJsonRpc> logger,
        IJsonRpcMessageHandler messageHandler,
        ISessionAuthenticator sessionAuthenticator)
            : base(messageHandler)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
    }

    protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
    {
        this.logger.LogTrace("Creating error details for exception: '{Name}'", exception.GetType().Name);

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
                this.logger.LogError(exception, "Unhandled exception processing RPC method '{Method}'.", request.Method);
                return new JsonRpcError.ErrorDetail()
                {
                    Code = JsonRpcErrorCode.InternalError,
                    Message = "An unexpected error occurred. Please try again later.",
                };
        }
    }

    protected override ValueTask<JsonRpcMessage> DispatchRequestAsync(JsonRpcRequest request, TargetMethod targetMethod, CancellationToken cancellationToken)
    {
        var methodInfo = targetMethod.TargetMethodInfo!;
        bool isAuthRequired = methodInfo.GetCustomAttribute<JsonRpcAuthorizeAttribute>() is not null;

        this.logger.LogTrace("Handling JSON-RPC 2.0 request: '{RequestName}'", methodInfo.Name);

        return isAuthRequired && !this.sessionAuthenticator.IsAuthenticated
            ? new ValueTask<JsonRpcMessage>(new JsonRpcError
            {
                Error = new JsonRpcError.ErrorDetail()
                {
                    Code = (JsonRpcErrorCode)401,
                    Message = "Authentication is required to perform this action.",
                }
            })
            : base.DispatchRequestAsync(request, targetMethod, cancellationToken);
    }
}