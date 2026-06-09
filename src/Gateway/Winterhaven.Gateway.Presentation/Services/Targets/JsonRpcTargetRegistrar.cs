using System;
using System.Collections.Generic;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation.Targets;

namespace Winterhaven.Gateway.Presentation.Services.Targets;

internal sealed class JsonRpcTargetRegistrar : IJsonRpcTargetRegistrar
{
    private readonly IEnumerable<IRpcTarget> targets;

    public JsonRpcTargetRegistrar(IEnumerable<IRpcTarget> targets)
    {
        this.targets = targets ?? throw new ArgumentNullException(nameof(targets));
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
