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

    private readonly IUserAccountContext userAccountContext;

    private readonly ISessionAuthenticator sessionAuthenticator;

    private SemaphoreSlim? loginStateLock = new(1, 1);

    private bool isDisposed;

    public UserAccountService(
        ILogger<UserAccountService> logger,
        IUserAccountClient userAccountClient,
        IUserAccountContext userAccountContext,
        ISessionAuthenticator sessionAuthenticator)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
        this.userAccountContext = userAccountContext ?? throw new ArgumentNullException(nameof(userAccountContext));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
    }

    ~UserAccountService()
    {
        this.Dispose(false);
    }

    public async Task<UserLoginResult> LoginUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserAccountService));

        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        // Ensure that only one login attempt can be in progress at a time to prevent race conditions.
        // This also ensures that if a user logs out while a login attempt is in progress, the login
        // attempt will fail instead of potentially leaving the session in an inconsistent state.
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

            // Authenticate the session with the access token.
            // This ensures that any HTTP requests made after this point will include the access token for authentication.
            this.sessionAuthenticator.Authenticate(response.AccessToken);

            this.logger.LogInformation("User login attempt completed for username '{Username}'", username);

            return new UserLoginResult(
                RefreshToken: response.RefreshToken);
        }
        finally
        {
            // Release the lock to allow other login attempts to proceed.
            // As well as logout attempts, which also locks to ensure they don't interfere with an in-progress login attempt.
            this.loginStateLock.Release();
        }
    }

    public async Task LogoutUserAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserAccountService));

        // Ensure that only one logout attempt can be in progress at a time.
        // This also ensures that if a user logs in while a logout attempt is in progress,
        // the login attempt will fail instead of potentially leaving the session in an inconsistent state.
        await this.loginStateLock!.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (!this.sessionAuthenticator.IsAuthenticated)
            {
                return;
            }

            string username = this.userAccountContext.Username!;

            this.logger.LogInformation("Attempting to log out user with username '{Username}'", username);

            await this.userAccountClient.LogoutUserAsync(cancellationToken).ConfigureAwait(false);
            this.sessionAuthenticator.Invalidate();

            this.logger.LogInformation("User logout attempt completed for username '{Username}'", username);
        }
        finally
        {
            // Release the lock to allow other logout or login attempts to proceed.
            this.loginStateLock.Release();
        }
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

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
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
