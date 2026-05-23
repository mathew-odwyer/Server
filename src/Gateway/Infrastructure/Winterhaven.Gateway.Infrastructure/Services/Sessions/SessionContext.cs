namespace Winterhaven.Gateway.Infrastructure.Services.Sessions;

using Microsoft.Extensions.Logging;
using System;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Domain.Events.Sessions;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

internal sealed class SessionContext : ISessionContext, ISessionAuthenticator
{
    private readonly ILogger<SessionContext> logger;

    public SessionContext(ILogger<SessionContext> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public event EventHandler<SessionAuthenticatedEventArgs>? SessionAuthenticated;

    public event EventHandler<SessionInvalidatedEventArgs>? SessionInvalidated;

    public event EventHandler<SessionRefreshedEventArgs>? SessionRefreshed;

    public bool IsAuthenticated
    {
        get { return this.Session != null; }
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

        this.SessionAuthenticated?.Invoke(this, new SessionAuthenticatedEventArgs(
            userAccountId: userSession.UserAccountId,
            username: userSession.Username,
            accessTokenExpiry: userSession.AccessTokenExpiry));
    }

    public void Invalidate()
    {
        if (!this.IsAuthenticated)
        {
            return;
        }

        var userAccountId = this.Session!.UserAccountId;
        string username = this.Session!.Username;

        this.Session = null;
        this.logger.LogDebug("Invalidated user session for username: '{Username}'", username);

        this.SessionInvalidated?.Invoke(this, new SessionInvalidatedEventArgs(
            userAccountId: userAccountId,
            username: username));
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

        this.SessionRefreshed?.Invoke(this, new SessionRefreshedEventArgs(
            userAccountId: userSession.UserAccountId,
            username: userSession.Username,
            accessTokenExpiry: userSession.AccessTokenExpiry));
    }
}