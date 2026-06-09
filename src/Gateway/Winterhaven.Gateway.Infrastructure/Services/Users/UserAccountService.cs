using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Users;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal sealed class UserAccountService : IUserAccountService
{
    private readonly ILogger<UserAccountService> logger;

    private readonly IMessageBus messageBus;

    private readonly IUserAccountClient userAccountClient;

    private readonly IUserSessionContext userSessionContext;

    private readonly IUserSessionManager userSessionManager;

    private readonly IUserTokenParser userTokenParser;

    public UserAccountService(
        ILogger<UserAccountService> logger,
        IUserAccountClient userAccountClient,
        IUserSessionManager userSessionManager,
        IUserSessionContext userSessionContext,
        IUserTokenParser userTokenParser,
        IMessageBus messageBus)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
        this.userSessionManager = userSessionManager ?? throw new ArgumentNullException(nameof(userSessionManager));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
        this.userTokenParser = userTokenParser ?? throw new ArgumentNullException(nameof(userTokenParser));
        this.messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
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
        if (this.userSessionContext.IsAuthenticated)
        {
            // Choose error over debug, we should know about it just to be safe.
            this.logger.LogError("An active session already exists for user: '{Username}'", username);
            throw new AuthorizationException("An active session already exists for this connection.");
        }

        this.logger.LogDebug("User logging in: '{Username}'", username);
        var response = await this.userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);

        //// Create the user session and authenticate the user.
        var userSession = this.userTokenParser.ParseUserToken(response.AccessToken);
        this.userSessionManager.EstablishUserSession(userSession);

        var notification = new UserLoggedInEvent(
            userAccountId: userSession.UserAccountId,
            accessToken: userSession.AccessToken);

        try
        {
            await this.messageBus.PublishAsync(
                data: notification,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (MessageBusException)
        {
            //// If for some reason we failed to publish on login, rollback and invalidate the user session.
            //// Re-throw to return an internal server error to the client.
            this.userSessionManager.InvalidateUserSession();
            throw;
        }

        this.logger.LogInformation("User logged in with ID: '{UserAccountId}'", userSession.UserAccountId);

        return new UserLoginResult(
            RefreshToken: response.RefreshToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (!this.userSessionContext.IsAuthenticated || this.userSessionContext.UserSession == null)
        {
            return;
        }

        var userAccountId = this.userSessionContext.UserSession.UserAccountId;
        string accessToken = this.userSessionContext.UserSession.AccessToken;

        this.logger.LogDebug("Attempting to log out user with ID: '{UserAccountId}'", userAccountId);

        var notification = new UserLoggedOutEvent(
            userAccountId: userAccountId,
            accessToken: accessToken);

        try
        {
            //// Attempt to logout and persist to the database.
            await this.userAccountClient.LogoutUserAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            //// Invalidate the session and publish an event regardless of whether we failed to save or not.
            //// This ensures that other services can still be notified regardless of persistence.
            //// Also, safe to call InvalidateUserSession here because we know for certain it's not disposed.
            this.userSessionManager.InvalidateUserSession();

            try
            {
                await this.messageBus.PublishAsync(
                    data: notification,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MessageBusException ex)
            {
                //// Catch and log here to prevent the MessageBusException from replacing the true exception.
                //// If something happened when attempting to logout and persist - we won't know what happened if we don't catch exceptoins here.
                this.logger.LogError(ex, "Failed to publish {Event} for user with ID: '{UserAccountId}'", nameof(UserLoggedOutEvent), userAccountId);
            }
        }

        this.logger.LogInformation("User logout attempt completed for user with ID: '{Username}'", userAccountId);
    }

    public async Task<UserRefreshTokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        //// This shouldn't happen unless someone is probing the server or
        //// the client has become majorly out of sync, either way big issue.
        if (!this.userSessionContext.IsAuthenticated || this.userSessionContext.UserSession == null)
        {
            // Choose warning over debug, we should know about it just to be safe.
            this.logger.LogWarning("User with ID '{UserAccountId}' attempted to refresh their session but is not authenticated.", this.userSessionContext.UserSession?.UserAccountId ?? Guid.Empty);
            throw new AuthorizationException("You must be logged in to refresh your session.");
        }

        var dto = new RefreshTokenRequestDto()
        {
            RefreshToken = refreshToken,
        };

        this.logger.LogDebug("Attempting to refresh user session for user with ID: '{UserAccountId}'", this.userSessionContext.UserSession.UserAccountId);

        var response = await this.userAccountClient.RefreshTokenAsync(dto, cancellationToken).ConfigureAwait(false);
        var newSession = this.userTokenParser.ParseUserToken(response.AccessToken);

        this.userSessionManager.RefreshUserSession(newSession);

        this.logger.LogInformation("User session refreshed for user with ID: '{Username}'", this.userSessionContext.UserSession.UserAccountId);

        return new UserRefreshTokenResult(
            RefreshToken: response.RefreshToken);
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

        this.logger.LogDebug("Registering potential user: '{Username}'", username);
        await this.userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);
        this.logger.LogInformation("User registered: '{Username}'", username);
    }
}
