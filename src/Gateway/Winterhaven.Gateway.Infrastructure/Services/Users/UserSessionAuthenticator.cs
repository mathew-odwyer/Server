using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal sealed class UserSessionAuthenticator : IUserSessionAuthenticator, IUserSessionContext, IUserSessionExpiryNotifier
{
    private readonly ILogger<UserSessionAuthenticator> logger;

    private bool isDisposed;

    private CancellationTokenSource? sessionTokenSource;

    public UserSessionAuthenticator(ILogger<UserSessionAuthenticator> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        sessionTokenSource = new CancellationTokenSource();
    }

    ~UserSessionAuthenticator()
    {
        Dispose(false);
    }

    public bool IsAuthenticated => UserSession != null;

    public CancellationToken SessionExpiredToken
    {
        get
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionAuthenticator));
            return sessionTokenSource!.Token;
        }
    }

    public UserSession? UserSession { get; private set; }

    public void Authenticate(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionAuthenticator));
        ArgumentNullException.ThrowIfNull(userSession);

        if (IsAuthenticated)
        {
            return;
        }

        UserSession = userSession;
        ScheduleExpiry();

        logger.LogDebug("Session authenticated: '{Username}'", UserSession.Username);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Invalidate()
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionAuthenticator));

        if (!IsAuthenticated)
        {
            return;
        }

        string username = UserSession!.Username;

        UserSession = null;

        // Cancel session expiration as we've invalidated.
        sessionTokenSource!.Cancel();

        logger.LogDebug("Session invalidated: '{Username}'", username);
    }

    public void Refresh(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionAuthenticator));
        ArgumentNullException.ThrowIfNull(userSession);

        if (!IsAuthenticated)
        {
            return;
        }

        if (sessionTokenSource!.IsCancellationRequested)
        {
            logger.LogWarning("Refresh arrived for '{Username}' but the session token has already expired. Connection is closing.", UserSession!.Username);
            return;
        }

        UserSession = userSession;
        ScheduleExpiry();

        logger.LogDebug("Session refreshed: '{Username}'", UserSession.Username);
    }

    private void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing && sessionTokenSource != null)
        {
            sessionTokenSource.Dispose();
            sessionTokenSource = null;
        }

        isDisposed = true;
    }

    private void ScheduleExpiry()
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionAuthenticator));

        if (!IsAuthenticated)
        {
            return;
        }

        // Determine how long we have to wait before session expiry.
        var delay = UserSession!.ExpiresAt - DateTimeOffset.UtcNow;

        logger.LogDebug("User session for user '{Username}' is expiring in: {Seconds}s", UserSession!.Username, delay.Seconds);

        // Schedule the cancellation token to expiry once the session expires.
        sessionTokenSource!.CancelAfter(delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
    }
}
