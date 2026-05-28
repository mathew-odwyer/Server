using System;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Targets.Health;

//// TODO: Make public and see if we can setup AsyncAPI.

internal sealed record HealthPingRpcParameters(
    double TimeStamp);

internal sealed record HealthPingRpcResult(
    double TimeStamp);

internal sealed record HealthHeartbeatRpcResult(
    bool IsAlive);

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
