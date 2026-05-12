namespace Winterhaven.Gateway.Presentation.Targets.Health;

using StreamJsonRpc;

internal sealed class HealthRpcTarget : RpcTargetBase
{
    [JsonRpcMethod("health.ping")]
    public static double Ping(double TimeStamp)
    {
        return TimeStamp;
    }

    [JsonRpcMethod("health.heartbeat")]
    public static bool Heartbeat()
    {
        return true;
    }
}