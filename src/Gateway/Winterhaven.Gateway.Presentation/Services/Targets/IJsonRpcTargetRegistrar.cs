using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Services.Targets;

internal interface IJsonRpcTargetRegistrar
{
    public void RegisterTargets(JsonRpc rpc);
}
