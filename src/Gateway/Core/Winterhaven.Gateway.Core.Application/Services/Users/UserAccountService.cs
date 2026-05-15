namespace Winterhaven.Gateway.Core.Application.Services.Users;

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Sessions;

internal sealed class UserAccountService : IUserAccountService
{
    private readonly ILogger<UserAccountService> logger;

    private readonly IUserAccountClient userAccountClient;

    private readonly ISessionAuthenticator sessionAuthenticator;

    public UserAccountService(ILogger<UserAccountService> logger, IUserAccountClient userAccountClient, ISessionAuthenticator sessionAuthenticator)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
    }

    public async Task<string> LoginUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        this.logger.LogInformation("Logging in user with username {Username}", username);

        var dto = new LoginUserRequestDto()
        {
            Username = username,
            Password = password,
        };

        // Ensure the API authenticates the user first.
        var resposne = await this.userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);

        // Then we can authenticate the session with the access token.
        this.sessionAuthenticator.Authenticate(resposne.AccessToken);

        return resposne.RefreshToken;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        // TODO: Log message with user information from the session.
        await this.userAccountClient.LogoutUserAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> RegisterUserAsync(string username, string password, string emailAddress, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);

        var dto = new RegisterUserRequestDto()
        {
            Username = username,
            Password = password,
            EmailAddress = emailAddress,
        };

        this.logger.LogDebug("Registering user with username {Username} and email address {EmailAddress}", username, emailAddress);

        await this.userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);

        return true;
    }
}