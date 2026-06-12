using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Presentation.Attributes;

namespace Winterhaven.Gateway.Presentation;

internal enum GatewayErrorCode
{
    ChatError = 1,
}

internal sealed class GatewayJsonRpc : JsonRpc
{
    private readonly ILogger<GatewayJsonRpc> logger;

    private readonly IUserSessionContext userSessionContext;

    public GatewayJsonRpc(
        ILogger<GatewayJsonRpc> logger,
        IUserSessionContext userSessionContext,
        IJsonRpcMessageHandler messageHandler)
        : base(messageHandler)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
    }

    protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
    {
        this.logger.LogTrace("Creating error details for exception: '{Name}'", exception.GetType().Name);

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

            case ChatMessageException chatMessageException:
                return new JsonRpcError.ErrorDetail()
                {
                    Code = (JsonRpcErrorCode)GatewayErrorCode.ChatError,
                    Message = chatMessageException.Message,
                };

            //// For everything else, log it and send nothing useful to the client.
            //// It's obviously a bug or some other issue - let's not leak any internals to the client.
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
        var methodInfo = targetMethod.TargetMethodInfo;
        bool isAuthRequired = methodInfo?.GetCustomAttribute<JsonRpcAuthorizeAttribute>() is not null;

        this.logger.LogTrace("Handling JSON-RPC 2.0 request: '{RequestName}'", methodInfo?.Name ?? "unknown");

        return isAuthRequired && !this.userSessionContext.IsAuthenticated
            ? throw new AuthorizationException("Authentication is required to perform this action.")
            : base.DispatchRequestAsync(request, targetMethod, cancellationToken);
    }
}
