using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Targets.Health;

[ExcludeFromCodeCoverage]
internal sealed record HealthPingRpcParameters(
    [property: JsonPropertyName("time_stamp")] double TimeStamp);

[ExcludeFromCodeCoverage]
internal sealed record HealthPingRpcResult(
    [property: JsonPropertyName("time_stamp")] double TimeStamp);

[ExcludeFromCodeCoverage]
internal sealed record HealthHeartbeatRpcResult(
    [property: JsonPropertyName("is_alive")] bool IsAlive);

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
