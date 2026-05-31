using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Winterhaven.Gateway.Core.Application.Services.Users;

namespace Winterhaven.Gateway.Presentation.Targets.Users;

[ExcludeFromCodeCoverage]
internal sealed record UserRegisterRpcParameters(
    string Username,
    string Password,
    string EmailAddress);

[ExcludeFromCodeCoverage]
internal sealed record UserRegisterRpcResult;

[ExcludeFromCodeCoverage]
internal sealed record UserLoginRpcParameters(
    string Username,
    string Password);

[ExcludeFromCodeCoverage]
internal sealed record UserLoginRpcResult(
    string RefreshToken);

[ExcludeFromCodeCoverage]
internal sealed record UserRefreshRpcParameters(
    string RefreshToken);

[ExcludeFromCodeCoverage]
internal sealed record UserRefreshRpcResult(
    string RefreshToken);

[ExcludeFromCodeCoverage]
internal sealed class UserRpcTarget : IRpcTarget
{
    private readonly IUserAccountService userAccountService;

    public UserRpcTarget(IUserAccountService userAccountService) =>
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));

    [JsonRpcMethod("user.login", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserLoginRpcResult> LoginAsync(UserLoginRpcParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var response = await userAccountService.LoginAsync(
            username: parameters.Username,
            password: parameters.Password,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new UserLoginRpcResult(
            RefreshToken: response.RefreshToken);
    }

    // TODO: Use [AuthorizeAttribute] for Refresh and Logout.
    [JsonRpcMethod("user.refresh", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserRefreshRpcResult> RefreshAsync(UserRefreshRpcParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var response = await userAccountService.RefreshTokenAsync(
            refreshToken: parameters.RefreshToken,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new UserRefreshRpcResult(
            RefreshToken: response.RefreshToken);
    }

    [JsonRpcMethod("user.register", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserRegisterRpcResult> RegisterAsync(UserRegisterRpcParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        await userAccountService.RegisterAsync(
            username: parameters.Username,
            password: parameters.Password,
            emailAddress: parameters.EmailAddress,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new UserRegisterRpcResult();
    }
}
