namespace Winterhaven.Gateway.Infrastructure.Services.Users;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Brokering;
using Winterhaven.Brokering.Events.Users;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

internal sealed class UserAccountService : IUserAccountService
{
    private readonly IEventPublisher eventPublisher;

    private readonly ILogger<UserAccountService> logger;

    private readonly ISessionAuthenticator sessionAuthenticator;

    private readonly ISessionContext sessionContext;

    private readonly IUserAccountClient userAccountClient;

    private bool isDisposed;

    private SemaphoreSlim? loginStateLock = new(1, 1);

    public UserAccountService(
        ILogger<UserAccountService> logger,
        IUserAccountClient userAccountClient,
        ISessionAuthenticator sessionAuthenticator,
        ISessionContext sessionContext,
        IEventPublisher eventPublisher)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
        this.sessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
        this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    ~UserAccountService()
    {
        this.Dispose(false);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<UserLoginResult> LoginUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserAccountService));
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        if (this.sessionAuthenticator.IsAuthenticated)
        {
            this.logger.LogDebug("Session already authenticated for user: '{Username}'", this.sessionContext.Session!.Username);
            throw new AuthorizationException("You must logout of your current session first.");
        }

        // Ensure that only one login attempt can be in progress at a time to prevent race conditions. This also ensures that if a user logs out while a login attempt is in progress, the login attempt will fail instead of potentially leaving the session in an inconsistent state.
        await this.loginStateLock!.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var dto = new LoginUserRequestDto()
            {
                Username = username,
                Password = password,
            };

            this.logger.LogInformation("Attempting to log in user with username '{Username}'", username);

            var response = await this.userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);

            // Authenticate the session with the access token. This ensures that any HTTP requests made after this point will include the access token for authentication.
            this.sessionAuthenticator.Authenticate(new UserSession(
                UserAccountId: ParseUserAccountId(response.AccessToken),
                Username: ParseUsername(response.AccessToken),
                AccessToken: response.AccessToken,
                RefreshToken: response.RefreshToken,
                AccessTokenExpiry: TimeSpan.FromSeconds(response.ExpirationSeconds)));

            await this.eventPublisher.PublishEventAsync("user.logged_in", new UserLoggedInEvent(
                Username: this.sessionContext.Session!.Username,
                AccessToken: this.sessionContext.Session!.AccessToken), cancellationToken).ConfigureAwait(false);

            this.logger.LogInformation("User login attempt completed for username '{Username}'", username);

            return new UserLoginResult(
                RefreshToken: response.RefreshToken);
        }
        finally
        {
            // Release the lock to allow other login attempts to proceed. As well as logout attempts, which also locks to ensure they don't interfere with an in-progress login attempt.
            this.loginStateLock.Release();
        }
    }

    public async Task LogoutUserAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserAccountService));

        // Ensure that only one logout attempt can be in progress at a time. This also ensures that if a user logs in while a logout attempt is in progress, the login attempt will fail instead of potentially leaving the session in an inconsistent state.
        await this.loginStateLock!.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (!this.sessionAuthenticator.IsAuthenticated)
            {
                return;
            }

            string username = this.sessionContext.Session!.Username!;

            this.logger.LogInformation("Attempting to log out user with username '{Username}'", username);

            await this.userAccountClient.LogoutUserAsync(cancellationToken).ConfigureAwait(false);
            this.sessionAuthenticator.Invalidate();

            await this.eventPublisher.PublishEventAsync("user.logged_out", new UserLoggedOutEvent(
                Username: username), cancellationToken).ConfigureAwait(false);

            this.logger.LogInformation("User logout attempt completed for username '{Username}'", username);
        }
        finally
        {
            // Release the lock to allow other logout or login attempts to proceed.
            this.loginStateLock.Release();
        }
    }

    public async Task<UserRefreshResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserAccountService));
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        if (!this.sessionAuthenticator.IsAuthenticated)
        {
            throw new AuthorizationException("You must be logged in to refresh your session.");
        }

        var dto = new RefreshTokenRequestDto()
        {
            RefreshToken = refreshToken,
        };

        this.logger.LogInformation("User refreshing tokens for username: '{Username}'", this.sessionContext.Session!.Username);

        var response = await this.userAccountClient.RefreshTokenAsync(dto, cancellationToken).ConfigureAwait(false);

        // Re-authenticate the session with the new access token.
        this.sessionAuthenticator.Refresh(new UserSession(
            UserAccountId: ParseUserAccountId(response.AccessToken),
            Username: ParseUsername(response.AccessToken),
            AccessToken: response.AccessToken,
            RefreshToken: response.RefreshToken,
            AccessTokenExpiry: TimeSpan.FromSeconds(response.ExpirationSeconds)));

        this.logger.LogInformation("User refreshed tokens for username: '{Username}'", this.sessionContext.Session!.Username);

        return new UserRefreshResult(
            RefreshToken: response.RefreshToken);
    }

    public async Task<UserRegistrationResult> RegisterUserAsync(string username, string password, string emailAddress, CancellationToken cancellationToken)
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

        this.logger.LogInformation("Attempting to register user with username '{Username}'", username);

        await this.userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);

        this.logger.LogInformation("User registration attempt completed for username '{Username}'", username);

        return new UserRegistrationResult(
            Success: true);
    }

    private static Guid ParseUserAccountId(string accessToken)
    {
        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(accessToken);
        return Guid.Parse(jwt.Claims.First(c => c.Type == "identifier").Value);
    }

    private static string ParseUsername(string accessToken)
    {
        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(accessToken);
        return jwt.Claims.First(c => c.Type == "username").Value;
    }

    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing && this.loginStateLock != null)
        {
            this.loginStateLock.Dispose();
            this.loginStateLock = null;
        }

        this.isDisposed = true;
    }
}