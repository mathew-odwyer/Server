namespace Winterhaven.Gateway.Presentation.Targets.Health;

using StreamJsonRpc;

internal sealed class HealthRpcTarget : RpcTargetBase
{
    [JsonRpcMethod("health.ping", UseSingleObjectParameterDeserialization = true)]
    public static double Ping(double timestamp)
    {
        return timestamp;
    }

    [JsonRpcMethod("health.heartbeat")]
    public static bool Heartbeat()
    {
        return true;
    }
}