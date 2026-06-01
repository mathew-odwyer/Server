using System;
using System.Threading;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation.Attributes;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Integrations.Services.Targets;

internal sealed class TestAuthRpcTarget : IRpcTarget
{
    [JsonRpcAuthorize]
    [JsonRpcMethod("auth.secret", UseSingleObjectParameterDeserialization = true)]
    public static string GetAuthenticatedResource() => "secret";

    [JsonRpcAuthorize]
    [JsonRpcMethod("auth.required", UseSingleObjectParameterDeserialization = true)]
    public static void RequireAuthentication(CancellationToken cancellationToken = default) => throw new InvalidOperationException();
}
