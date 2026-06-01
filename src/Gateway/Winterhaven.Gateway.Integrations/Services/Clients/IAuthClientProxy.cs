using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Integrations.Services.Clients;

internal interface IAuthClientProxy
{
    [JsonRpcMethod("auth.secret", UseSingleObjectParameterDeserialization = true)]
    public Task<string> GetUserSecret(CancellationToken cancellationToken = default);

    [JsonRpcMethod("auth.required", UseSingleObjectParameterDeserialization = true)]
    public Task RequireAuthentication(CancellationToken cancellationToken = default);
}
