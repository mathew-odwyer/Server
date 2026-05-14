namespace Winterhaven.Gateway.Presentation.Targets.Health;

using AutoMapper;
using MediatR;
using StreamJsonRpc;
using System;
using Winterhaven.Gateway.Presentation.DTOs.Health;

internal sealed class HealthRpcTarget : RpcTargetBase
{
    public HealthRpcTarget(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [JsonRpcMethod("health.ping", UseSingleObjectParameterDeserialization = true)]
    public static double Ping(HealthPingRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return request.TimeStamp;
    }

    [JsonRpcMethod("health.heartbeat")]
    public static bool Heartbeat()
    {
        return true;
    }
}