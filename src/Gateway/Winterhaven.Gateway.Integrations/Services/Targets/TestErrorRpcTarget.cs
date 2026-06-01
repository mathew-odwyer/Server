using System;
using System.Collections.Generic;
using System.Threading;
using StreamJsonRpc;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Integrations.Services.Targets;

internal sealed class TestErrorRpcTarget : IRpcTarget
{
    [JsonRpcMethod("error.unauthorized", UseSingleObjectParameterDeserialization = true)]
    public static void GenerateUnauthorizedError(CancellationToken cancellationToken = default) =>
        throw new AuthorizationException();

    [JsonRpcMethod("error.unhandled", UseSingleObjectParameterDeserialization = true)]
    public static void GenerateUnhandledError(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException();

    [JsonRpcMethod("error.validation", UseSingleObjectParameterDeserialization = true)]
    public static void GenerateValidationError(CancellationToken cancellationToken = default) =>
        throw new ValidationException(new Dictionary<string, string[]>
        {
            { "General", ["One or more validation errors occurred."] },
            { "Other", ["This is a cool test lol", "seriously cool"] },
        });

    [JsonRpcMethod("error.validation.null", UseSingleObjectParameterDeserialization = true)]
    public static void GenerateValidationErrorWithNullErrors(CancellationToken cancellationToken = default) =>
        throw new ValidationException(errors: null);
}
