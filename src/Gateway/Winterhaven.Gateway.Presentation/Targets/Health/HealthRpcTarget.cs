using System;
using System.Diagnostics.CodeAnalysis;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Targets.Health;

[ExcludeFromCodeCoverage]
internal sealed record HealthPingRpcParameters(
    double TimeStamp);

[ExcludeFromCodeCoverage]
internal sealed record HealthPingRpcResult(
    double TimeStamp);

[ExcludeFromCodeCoverage]
internal sealed record HealthHeartbeatRpcResult(
    bool IsAlive);

[ExcludeFromCodeCoverage]
internal sealed class HealthRpcTarget : IRpcTarget
{
    [JsonRpcMethod("health.heartbeat")]
    public static HealthHeartbeatRpcResult Heartbeat() => new(IsAlive: true);

    [JsonRpcMethod("health.ping", UseSingleObjectParameterDeserialization = true)]
    public static HealthPingRpcResult Ping(HealthPingRpcParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return new HealthPingRpcResult(
            TimeStamp: parameters.TimeStamp);
    }
}
