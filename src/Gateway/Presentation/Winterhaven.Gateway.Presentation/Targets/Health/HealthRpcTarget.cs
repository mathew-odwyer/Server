namespace Winterhaven.Gateway.Presentation.Targets.Health;

using StreamJsonRpc;
using System;
using Winterhaven.Gateway.Presentation.Validation;

public sealed record HealthPingRpcParameters(
    double TimeStamp);

public sealed record HealthPingRpcResult(
    double TimeStamp);

public sealed record HealthHeartbeatRpcResult(
    bool IsAlive);

public sealed class HealthRpcTarget : RpcTargetBase
{
    private readonly IValidatorFactory validatorFactory;

    public HealthRpcTarget(IValidatorFactory validatorFactory)
    {
        this.validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
    }

    [JsonRpcMethod("health.ping", UseSingleObjectParameterDeserialization = true)]
    public HealthPingRpcResult Ping(HealthPingRpcParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        Validator.Validate(this.validatorFactory, parameters);

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
