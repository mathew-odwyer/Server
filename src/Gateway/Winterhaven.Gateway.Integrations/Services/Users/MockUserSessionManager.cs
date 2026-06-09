using System;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Integrations.Services.Users;

internal sealed class MockUserSessionManager : IUserSessionManager, IUserSessionContext
{
    static MockUserSessionManager()
    {
        DummySession = new UserSession(
        UserAccountId: Guid.NewGuid(),
        AccessToken: "accessToken",
        ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15));
    }

    public event EventHandler<EventArgs> Established;

    public event EventHandler<EventArgs> Invalidated;

    public event EventHandler<EventArgs> Refreshed;

    public static UserSession DummySession { get; }

    public bool IsAuthenticated
    {
        get
        {
            return this.UserSession != null;
        }
    }

    public UserSession UserSession { get; private set; }

    public void Dispose()
    {
    }

    public void EstablishUserSession(UserSession userSession)
    {
        ArgumentNullException.ThrowIfNull(userSession);

        this.UserSession = userSession;
        Established?.Invoke(this, EventArgs.Empty);
    }

    public void InvalidateUserSession()
    {
        this.UserSession = null;
        Invalidated?.Invoke(this, EventArgs.Empty);
    }

    public void RefreshUserSession(UserSession userSession)
    {
        this.UserSession = userSession;
        Refreshed?.Invoke(this, EventArgs.Empty);
    }
}
