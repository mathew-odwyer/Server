using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal sealed class UserSessionManager : IUserSessionManager, IUserSessionContext
{
    private readonly ILogger<UserSessionManager> logger;

    private readonly TimeProvider timeProvider;

    private readonly ITimer timer;

    private bool isDisposed;

    public UserSessionManager(ILogger<UserSessionManager> logger, TimeProvider timeProvider)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

        timer = timeProvider.CreateTimer(
            callback: _ => InvalidateUserSession(),
            state: null,
            dueTime: Timeout.InfiniteTimeSpan,
            period: Timeout.InfiniteTimeSpan);
    }

    ~UserSessionManager()
    {
        Dispose(false);
    }

    public event EventHandler<EventArgs>? Established;

    public event EventHandler<EventArgs>? Invalidated;

    public event EventHandler<EventArgs>? Refreshed;

    public bool IsAuthenticated => UserSession != null;

    public UserSession? UserSession { get; private set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void EstablishUserSession(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));
        ArgumentNullException.ThrowIfNull(userSession);

        if (IsAuthenticated)
        {
            return;
        }

        if (userSession.ExpiresAt <= timeProvider.GetUtcNow())
        {
            logger.LogWarning("Attempted to establish an already-expired session for '{Username}'", userSession.Username);
            return;
        }

        UserSession = userSession;
        StartExpiryTimer();

        Established?.Invoke(this, EventArgs.Empty);
        logger.LogDebug("Session authenticated: '{Username}'", UserSession.Username);
    }

    public void InvalidateUserSession()
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));

        if (!IsAuthenticated)
        {
            return;
        }

        string username = UserSession!.Username;

        StopExpiryTimer();
        UserSession = null;

        Invalidated?.Invoke(this, EventArgs.Empty);

        logger.LogInformation("Session invalidated: '{Username}'", username);
    }

    public void RefreshUserSession(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));
        ArgumentNullException.ThrowIfNull(userSession);

        if (!IsAuthenticated)
        {
            return;
        }

        if (userSession.ExpiresAt <= timeProvider.GetUtcNow())
        {
            logger.LogWarning("Attempted to refresh with an already-expired session for '{Username}'", userSession.Username);
            return;
        }

        UserSession = userSession;
        ResetExpiryTimer();
        Refreshed?.Invoke(this, EventArgs.Empty);

        logger.LogInformation("Session refreshed: '{Username}'", UserSession.Username);
    }

    private void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing && timer != null)
        {
            timer.Dispose();
        }

        isDisposed = true;
    }

    private void ResetExpiryTimer()
    {
        StopExpiryTimer();
        StartExpiryTimer();
    }

    private void StartExpiryTimer()
    {
        var delay = UserSession!.ExpiresAt - timeProvider.GetUtcNow();

        timer.Change(delay, Timeout.InfiniteTimeSpan);

        logger.LogDebug(
            "User session for '{Username}' expires in {Seconds}s",
            UserSession.Username,
            delay.TotalSeconds);
    }

    private void StopExpiryTimer()
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));
        timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }
}
