using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Winterhaven.Gateway.Core.Application.Services.Users;

namespace Winterhaven.Gateway.Presentation.Targets.Users;

internal sealed record UserRegisterRpcParameters(
    string Username,
    string Password,
    string EmailAddress);

internal sealed record UserRegisterRpcResult(
    bool Success);

[ExcludeFromCodeCoverage]
internal sealed class UserRpcTarget : IRpcTarget
{
    private readonly IUserAccountService userAccountService;

    public UserRpcTarget(IUserAccountService userAccountService) =>
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));

    [JsonRpcMethod("user.register", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserRegisterRpcResult> RegisterAsync(UserRegisterRpcParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var response = await userAccountService.RegisterAsync(
            username: parameters.Username,
            password: parameters.Password,
            emailAddress: parameters.EmailAddress,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new UserRegisterRpcResult(
            Success: response.Success);
    }
}
