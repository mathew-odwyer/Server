namespace Winterhaven.Gateway.Presentation.Targets.Users;

using StreamJsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Attributes;
using Winterhaven.Gateway.Presentation.Validation;

internal sealed record UserLoginParameters(
    string Username,
    string Password);

internal sealed record UserRegisterParameters(
    string Username,
    string Password,
    string EmailAddress);

internal sealed class UserRpcTarget : RpcTargetBase
{
    private readonly IUserAccountService userAccountService;

    private readonly ISessionAuthenticator sessionAuthenticator;

    private readonly IValidatorFactory validatorFactory;

    public UserRpcTarget(IUserAccountService userAccountService, ISessionAuthenticator sessionAuthenticator, IValidatorFactory validatorFactory)
    {
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
        this.validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("user.logout")]
    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await this.userAccountService.LogoutAsync(cancellationToken).ConfigureAwait(false);

        // TODO: Figure out how to disconnect from the gateway after logout. Maybe we can have a
        // IConnectionContext that we can use to disconnect the client after logout. - Disconnection
        // is a presentation concern, so it should be handled in the presentation layer.
    }

    [JsonRpcMethod("user.register", UseSingleObjectParameterDeserialization = true)]
    public async Task<bool> RegisterAsync(UserRegisterParameters parameters, CancellationToken cancellationToken)
    {
        Validator.Validate(this.validatorFactory, parameters);
        return await this.userAccountService.RegisterUserAsync(parameters.Username, parameters.Password, parameters.EmailAddress, cancellationToken).ConfigureAwait(false);
    }

    [JsonRpcMethod("user.login", UseSingleObjectParameterDeserialization = true)]
    public async Task<string> LoginAsync(UserLoginParameters parameters, CancellationToken cancellationToken)
    {
        Validator.Validate(this.validatorFactory, parameters);
        return await this.userAccountService.LoginUserAsync(parameters.Username, parameters.Password, cancellationToken).ConfigureAwait(false);
    }
}