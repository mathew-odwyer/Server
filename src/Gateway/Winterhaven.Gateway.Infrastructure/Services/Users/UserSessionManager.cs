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

        this.timer = timeProvider.CreateTimer(
            callback: _ => this.InvalidateUserSession(),
            state: null,
            dueTime: Timeout.InfiniteTimeSpan,
            period: Timeout.InfiniteTimeSpan);
    }

    ~UserSessionManager()
    {
        this.Dispose(false);
    }

    public event EventHandler<EventArgs>? Established;

    public event EventHandler<EventArgs>? Invalidated;

    public event EventHandler<EventArgs>? Refreshed;

    public bool IsAuthenticated
    {
        get
        {
            return this.UserSession != null;
        }
    }

    public UserSession? UserSession { get; private set; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void EstablishUserSession(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserSessionManager));
        ArgumentNullException.ThrowIfNull(userSession);

        if (this.IsAuthenticated)
        {
            return;
        }

        this.logger.LogDebug("Establishing session for user with ID: '{UserAccountId}'", userSession.UserAccountId);

        if (userSession.ExpiresAt <= this.timeProvider.GetUtcNow())
        {
            this.logger.LogWarning("Attempting to refresh user session for user with ID: '{UserAccountId}'", userSession.UserAccountId);
            return;
        }

        this.UserSession = userSession;
        this.StartExpiryTimer();

        Established?.Invoke(this, EventArgs.Empty);
        this.logger.LogInformation("User session established for user with ID: '{UserAccountId}'", this.UserSession.UserAccountId);
    }

    public void InvalidateUserSession()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserSessionManager));

        if (!this.IsAuthenticated)
        {
            return;
        }

        var userAccountId = this.UserSession!.UserAccountId;

        this.logger.LogDebug("Invalidating user session for user with ID: '{UserAccountId}'", userAccountId);

        this.StopExpiryTimer();
        this.UserSession = null;

        Invalidated?.Invoke(this, EventArgs.Empty);

        this.logger.LogInformation("User session invalidated for user with ID: '{UserAccountId}'", userAccountId);
    }

    public void RefreshUserSession(UserSession userSession)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserSessionManager));
        ArgumentNullException.ThrowIfNull(userSession);

        if (!this.IsAuthenticated)
        {
            return;
        }

        var userAccountId = userSession!.UserAccountId;

        this.logger.LogDebug("Refresh user session for user with ID: '{UserAccountId}'", userAccountId);

        if (userSession.ExpiresAt <= this.timeProvider.GetUtcNow())
        {
            this.logger.LogWarning("Attempted to refresh with an already-expired session for user with ID: '{UserAccountId}'", userAccountId);
            return;
        }

        this.UserSession = userSession;
        this.ResetExpiryTimer();
        Refreshed?.Invoke(this, EventArgs.Empty);

        this.logger.LogInformation("User session refreshed for user with ID: '{UserAccountId}'", userAccountId);
    }

    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing && this.timer != null)
        {
            this.timer.Dispose();
        }

        this.isDisposed = true;
    }

    private void ResetExpiryTimer()
    {
        this.StopExpiryTimer();
        this.StartExpiryTimer();
    }

    private void StartExpiryTimer()
    {
        var delay = this.UserSession!.ExpiresAt - this.timeProvider.GetUtcNow();
        this.timer.Change(delay, Timeout.InfiniteTimeSpan);

        this.logger.LogInformation("User session for user with ID '{UserAccountId}' expires in {Seconds}s", this.UserSession.UserAccountId, delay.TotalSeconds);
    }

    private void StopExpiryTimer()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(UserSessionManager));
        this.timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }
}
