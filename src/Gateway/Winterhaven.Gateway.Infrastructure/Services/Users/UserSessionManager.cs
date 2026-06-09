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

        logger.LogDebug("Establishing session for user with ID: '{UserAccountId}'", userSession.UserAccountId);

        if (userSession.ExpiresAt <= timeProvider.GetUtcNow())
        {
            logger.LogWarning("Attempting to refresh user session for user with ID: '{UserAccountId}'", userSession.UserAccountId);
            return;
        }

        UserSession = userSession;
        StartExpiryTimer();

        Established?.Invoke(this, EventArgs.Empty);
        logger.LogInformation("User session established for user with ID: '{UserAccountId}'", UserSession.UserAccountId);
    }

    public void InvalidateUserSession()
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));

        if (!IsAuthenticated)
        {
            return;
        }

        var userAccountId = UserSession!.UserAccountId;

        logger.LogDebug("Invalidating user session for user with ID: '{UserAccountId}'", userAccountId);

        StopExpiryTimer();
        UserSession = null;

        Invalidated?.Invoke(this, EventArgs.Empty);

        logger.LogInformation("User session invalidated for user with ID: '{UserAccountId}'", userAccountId);
    }

    public void RefreshUserSession(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));
        ArgumentNullException.ThrowIfNull(userSession);

        if (!IsAuthenticated)
        {
            return;
        }

        var userAccountId = userSession!.UserAccountId;

        logger.LogDebug("Refresh user session for user with ID: '{UserAccountId}'", userAccountId);

        if (userSession.ExpiresAt <= timeProvider.GetUtcNow())
        {
            logger.LogWarning("Attempted to refresh with an already-expired session for user with ID: '{UserAccountId}'", userAccountId);
            return;
        }

        UserSession = userSession;
        ResetExpiryTimer();
        Refreshed?.Invoke(this, EventArgs.Empty);

        logger.LogInformation("User session refreshed for user with ID: '{UserAccountId}'", userAccountId);
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

        logger.LogInformation("User session for user with ID '{UserAccountId}' expires in {Seconds}s", UserSession.UserAccountId, delay.TotalSeconds);
    }

    private void StopExpiryTimer()
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(UserSessionManager));
        timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }
}
