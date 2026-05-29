using System;
using StreamJsonRpc;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Integrations.Presentation.Targets;

internal sealed class ErrorThrowingTestTarget : IRpcTarget
{
    [JsonRpcMethod("test.throwAuthorization")]
    public static void ThrowAuthorization() =>
        throw new AuthorizationException("Authorization failed.");

    [JsonRpcMethod("test.throwUnhandled")]
    public static void ThrowUnhandled() =>
        throw new InvalidOperationException("Something went wrong.");

    [JsonRpcMethod("test.throwValidation")]
    public static void ThrowValidation() =>
        throw new ValidationException("Validation failed.");
}
