namespace Winterhaven.Gateway.Presentation.Targets.Services;

using StreamJsonRpc;
using System;
using System.Collections.Generic;

internal sealed class JsonRpcRegistrar
{
    private readonly IEnumerable<RpcTargetBase> targets;

    public JsonRpcRegistrar(IEnumerable<RpcTargetBase> targets)
    {
        this.targets = targets ?? throw new System.ArgumentNullException(nameof(targets));
    }

    public void RegisterTargets(JsonRpc rpc)
    {
        ArgumentNullException.ThrowIfNull(rpc);

        foreach (var target in this.targets)
        {
            rpc.AddLocalRpcTarget(target);
        }
    }
}