using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Winterhaven.Brokering;
using Winterhaven.Brokering.Events.Users;
using Winterhaven.Brokering.Exceptions;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;

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
        if (userSessionContext.IsAuthenticated)
        {
            // Choose error over debug, we should know about it just to be safe.
            logger.LogError("An active session already exists for user: '{Username}'", username);
            throw new AuthorizationException("An active session already exists for this connection.");
        }

        logger.LogDebug("User logging in: '{Username}'", username);
        var response = await userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);

        //// Create the user session and authenticate the user.
        var userSession = userTokenParser.ParseUserToken(response.AccessToken);
        userSessionManager.EstablishUserSession(userSession);

        var notification = new UserLoggedInEvent(
            userAccountId: userSession.UserAccountId,
            accessToken: userSession.AccessToken);

        try
        {
            await messageBus.PublishAsync(
                data: notification,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (MessageBusException)
        {
            //// If for some reason we failed to publish on login, rollback and invalidate the user session.
            //// Re-throw to return an internal server error to the client.
            userSessionManager.InvalidateUserSession();
            throw;
        }

        logger.LogInformation("User logged in with ID: '{UserAccountId}'", userSession.UserAccountId);

        return new UserLoginResult(
            RefreshToken: response.RefreshToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (!userSessionContext.IsAuthenticated || userSessionContext.UserSession == null)
        {
            return;
        }

        var userAccountId = userSessionContext.UserSession.UserAccountId;
        string accessToken = userSessionContext.UserSession.AccessToken;

        logger.LogDebug("Attempting to log out user with ID: '{UserAccountId}'", userAccountId);

        var notification = new UserLoggedOutEvent(
            userAccountId: userAccountId,
            accessToken: accessToken);

        try
        {
            //// Attempt to logout and persist to the database.
            await userAccountClient.LogoutUserAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            //// Invalidate the session and publish an event regardless of whether we failed to save or not.
            //// This ensures that other services can still be notified regardless of persistence.
            //// Also, safe to call InvalidateUserSession here because we know for certain it's not disposed.
            userSessionManager.InvalidateUserSession();

            try
            {
                await messageBus.PublishAsync(
                    data: notification,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MessageBusException ex)
            {
                //// Catch and log here to prevent the MessageBusException from replacing the true exception.
                //// If something happened when attempting to logout and persist - we won't know what happened if we don't catch exceptoins here.
                logger.LogError(ex, "Failed to publish {Event} for user with ID: '{UserAccountId}'", nameof(UserLoggedOutEvent), userAccountId);
            }
        }

        logger.LogInformation("User logout attempt completed for user with ID: '{Username}'", userAccountId);
    }

    public async Task<UserRefreshTokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        //// This shouldn't happen unless someone is probing the server or
        //// the client has become majorly out of sync, either way big issue.
        if (!userSessionContext.IsAuthenticated || userSessionContext.UserSession == null)
        {
            // Choose warning over debug, we should know about it just to be safe.
            logger.LogWarning("User with ID '{UserAccountId}' attempted to refresh their session but is not authenticated.", userSessionContext.UserSession?.UserAccountId ?? Guid.Empty);
            throw new AuthorizationException("You must be logged in to refresh your session.");
        }

        var dto = new RefreshTokenRequestDto()
        {
            RefreshToken = refreshToken,
        };

        logger.LogDebug("Attempting to refresh user session for user with ID: '{UserAccountId}'", userSessionContext.UserSession.UserAccountId);

        var response = await userAccountClient.RefreshTokenAsync(dto, cancellationToken).ConfigureAwait(false);
        var newSession = userTokenParser.ParseUserToken(response.AccessToken);

        userSessionManager.RefreshUserSession(newSession);

        logger.LogInformation("User session refreshed for user with ID: '{Username}'", userSessionContext.UserSession.UserAccountId);

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

        logger.LogDebug("Registering potential user: '{Username}'", username);
        await userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("User registered: '{Username}'", username);
    }
}
