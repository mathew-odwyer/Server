namespace Winterhaven.Gateway.Presentation.Targets.Users;

using StreamJsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Attributes;
using Winterhaven.Gateway.Presentation.Validation;

public sealed record UserLoginRpcParameters(
    string Username,
    string Password);

public sealed record UserRegisterRpcParameters(
    string Username,
    string Password,
    string EmailAddress);

public sealed record UserLoginRpcResult(
    string RefreshToken);

public sealed record UserRegisterRpcResult(
    bool Success);

public sealed record UserLogoutRpcResult(
    bool Success);

public sealed record UserRefreshRpcParameters(
    string RefreshToken);

public sealed record UserRefreshRpcResult(
    string RefreshToken);

public sealed class UserRpcTarget : RpcTargetBase
{
    private readonly IUserAccountService userAccountService;

    private readonly IValidatorFactory validatorFactory;

    public UserRpcTarget(IUserAccountService userAccountService, IValidatorFactory validatorFactory)
    {
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
        this.validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("user.refresh", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserRefreshRpcResult> RefreshAsync(UserRefreshRpcParameters parameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        Validator.Validate(this.validatorFactory, parameters);

        var response = await this.userAccountService.RefreshTokenAsync(parameters.RefreshToken, cancellationToken).ConfigureAwait(false);

        return new UserRefreshRpcResult(
            RefreshToken: response.RefreshToken);
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("user.logout")]
    public async Task<UserLogoutRpcResult> LogoutAsync(CancellationToken cancellationToken)
    {
        await this.userAccountService.LogoutUserAsync(cancellationToken).ConfigureAwait(false);

        // There's no need to disconnect the client when they logout explicitly.
        // The client would be returned to the main menu and can choose to login again or disconnect.
        return new UserLogoutRpcResult(
            Success: true);
    }

    [JsonRpcMethod("user.register", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserRegisterRpcResult> RegisterAsync(UserRegisterRpcParameters parameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        Validator.Validate(this.validatorFactory, parameters);

        var response = await this.userAccountService.RegisterUserAsync(parameters.Username, parameters.Password, parameters.EmailAddress, cancellationToken).ConfigureAwait(false);

        return new UserRegisterRpcResult(
            Success: response.Success);
    }

    [JsonRpcMethod("user.login", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserLoginRpcResult> LoginAsync(UserLoginRpcParameters parameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        Validator.Validate(this.validatorFactory, parameters);

        var response = await this.userAccountService.LoginUserAsync(parameters.Username, parameters.Password, cancellationToken).ConfigureAwait(false);

        return new UserLoginRpcResult(
            RefreshToken: response.RefreshToken);
    }
}