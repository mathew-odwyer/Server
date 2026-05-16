namespace Winterhaven.Gateway.Core.Application.Services.Sessions;

using System;
using Winterhaven.Gateway.Core.Domain.Events.Sessions;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

public interface ISessionAuthenticator
{
    event EventHandler<SessionAuthenticatedEventArgs>? SessionAuthenticated;

    event EventHandler<SessionAuthenticatedEventArgs>? SessionRefreshed;

    bool IsAuthenticated { get; }

    void Refresh(UserSession userSession);

    void Authenticate(UserSession usersSession);

    void Invalidate();
}