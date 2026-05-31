using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal sealed class UserAccountService : IUserAccountService
{
    private readonly ILogger<UserAccountService> logger;

    private readonly IUserAccountClient userAccountClient;

    private readonly IUserSessionAuthenticator userSessionAuthenticator;

    private readonly IUserSessionContext userSessionContext;

    private readonly IUserTokenParser userTokenParser;

    public UserAccountService(
        ILogger<UserAccountService> logger,
        IUserAccountClient userAccountClient,
        IUserSessionAuthenticator userSessionAuthenticator,
        IUserSessionContext userSessionContext,
        IUserTokenParser userTokenParser)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
        this.userSessionAuthenticator = userSessionAuthenticator ?? throw new ArgumentNullException(nameof(userSessionAuthenticator));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
        this.userTokenParser = userTokenParser ?? throw new ArgumentNullException(nameof(userTokenParser));
    }

    public async Task<UserLoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var dto = new LoginUserRequestDto()
        {
            Username = username,
            Password = password,
        };

        //// If some how this user session is already authenticated, let's just return a 401
        //// as this should never really happen in production unless there's a bug or someone
        //// is probing the server.
        if (userSessionAuthenticator.IsAuthenticated)
        {
            logger.LogError("An active session already exists for user: '{Username}'", username);
            throw new AuthorizationException("An active session already exists for this connection.");
        }

        logger.LogInformation("User logging in: '{Username}'", username);
        var response = await userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);

        //// Create the user session and authenticate the user.
        var userSession = userTokenParser.ParseUserToken(response.AccessToken);
        userSessionAuthenticator.Authenticate(userSession);

        logger.LogInformation("User logged in: '{Username}'", username);

        return new UserLoginResult(
            RefreshToken: response.RefreshToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (!userSessionAuthenticator.IsAuthenticated)
        {
            return;
        }

        string username = userSessionContext.UserSession!.Username;

        logger.LogInformation("Attempting to log out user with username '{Username}'", username);

        await userAccountClient.LogoutUserAsync(cancellationToken).ConfigureAwait(false);
        userSessionAuthenticator.Invalidate();

        logger.LogInformation("User logout attempt completed for username '{Username}'", username);
    }

    public async Task RegisterAsync(string username, string password, string emailAddress, CancellationToken cancellationToken = default)
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

        logger.LogInformation("Registering potential user: '{Username}'", username);
        await userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Registered potential user: '{Username}'", username);
    }
}
