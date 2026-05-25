namespace Winterhaven.Gateway.Infrastructure.Services.Sessions;

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

internal sealed class SessionContext : ISessionContext, ISessionAuthenticator, IDisposable
{
    private readonly ILogger<SessionContext> logger;

    private bool isDisposed;

    private SemaphoreSlim? semaphore;

    public SessionContext(ILogger<SessionContext> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.semaphore = new SemaphoreSlim(0, 1);
    }

    ~SessionContext()
    {
        this.Dispose(false);
    }

    public bool IsAuthenticated
    {
        get { return !this.isDisposed && this.Session != null; }
    }

    public UserSession? Session { get; private set; }

    public void Authenticate(UserSession userSession)
    {
        ArgumentNullException.ThrowIfNull(userSession);

        if (this.IsAuthenticated)
        {
            return;
        }

        this.Session = userSession;
        this.logger.LogDebug("User session authenticated: '{Username}'", this.Session.Username);
        this.semaphore!.Release();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Invalidate()
    {
        if (!this.IsAuthenticated)
        {
            return;
        }

        string username = this.Session!.Username;

        this.Session = null;
        this.logger.LogDebug("Invalidated user session for username: '{Username}'", username);
    }

    public void Refresh(UserSession userSession)
    {
        ArgumentNullException.ThrowIfNull(userSession);

        if (!this.IsAuthenticated)
        {
            return;
        }

        this.Session = userSession;
        this.logger.LogDebug("User session refreshed: '{Username}'", this.Session.Username);
    }

    public async Task<UserSession> WaitForAuthenticationAsync(CancellationToken cancellationToken)
    {
        if (this.IsAuthenticated)
        {
            return this.Session!;
        }

        this.logger.LogDebug("Waiting for session authentication...");
        await this.semaphore!.WaitAsync(cancellationToken).ConfigureAwait(false);

        return this.Session!;
    }

    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            if (semaphore != null)
            {
                semaphore.Dispose();
                semaphore = null;
            }
        }

        this.isDisposed = true;
    }
}