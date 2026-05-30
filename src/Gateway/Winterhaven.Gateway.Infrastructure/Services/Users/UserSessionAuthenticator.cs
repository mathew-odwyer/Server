using System;
using Microsoft.Extensions.Logging;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal sealed class UserSessionAuthenticator : IUserSessionAuthenticator, IUserSessionContext
{
    private readonly ILogger<UserSessionAuthenticator> logger;

    public UserSessionAuthenticator(ILogger<UserSessionAuthenticator> logger) =>
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public bool IsAuthenticated => UserSession != null;

    public UserSession? UserSession { get; private set; }

    public void Authenticate(UserSession userSession)
    {
        ArgumentNullException.ThrowIfNull(userSession);

        if (IsAuthenticated)
        {
            return;
        }

        UserSession = userSession;

        logger.LogDebug("Session authenticated: '{Username}'", UserSession.Username);
    }

    public void Invalidate()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        string username = UserSession!.Username;

        UserSession = null;
        logger.LogDebug("Session invalidated: '{Username}'", username);
    }

    public void Refresh(UserSession userSession)
    {
        ArgumentNullException.ThrowIfNull(userSession);

        if (!IsAuthenticated)
        {
            return;
        }

        UserSession = userSession;
        logger.LogDebug("Session refreshed: '{Username}'", UserSession.Username);
    }
}
