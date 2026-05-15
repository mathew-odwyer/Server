namespace Winterhaven.Gateway.Presentation.Targets.Health;

using StreamJsonRpc;

internal sealed record HealthPingRpcParameters(
    double TimeStamp);

internal sealed record HealthPingRpcResult(
    double TimeStamp);

internal sealed record HealthHeartbeatRpcResult(
    bool IsAlive);

internal sealed class HealthRpcTarget : RpcTargetBase
{
    [JsonRpcMethod("health.ping", UseSingleObjectParameterDeserialization = true)]
    public static HealthPingRpcResult Ping(HealthPingRpcParameters parameters)
    {
        return new HealthPingRpcResult(
            TimeStamp: parameters.TimeStamp);
    }

    [JsonRpcMethod("health.heartbeat")]
    public static HealthHeartbeatRpcResult Heartbeat()
    {
        return new HealthHeartbeatRpcResult(
            IsAlive: true);
    }
}
