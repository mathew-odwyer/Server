using System.Threading;
using System.Threading.Tasks;
using PolyType;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation.Targets.Users;

namespace Winterhaven.Gateway.Integrations.Services.Clients;

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
internal partial interface IUserClientProxy
{
    [JsonRpcMethod("user.login", ClientRequiresNamedArguments = true)]
    public Task<UserLoginRpcResult> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("user.refresh")]
    public Task<UserRefreshRpcResult> RefreshAsync(
        string refrshToken,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("user.register", ClientRequiresNamedArguments = true)]
    public Task<UserRegisterRpcResult> RegisterAsync(
        string username,
        string password,
        string email_address,
        CancellationToken cancellationToken = default);
}
