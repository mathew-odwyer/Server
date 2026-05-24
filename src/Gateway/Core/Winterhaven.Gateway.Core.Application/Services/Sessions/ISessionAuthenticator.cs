namespace Winterhaven.Gateway.Core.Application.Services.Sessions;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

public interface ISessionAuthenticator
{
    bool IsAuthenticated { get; }

    void Authenticate(UserSession usersSession);

    void Invalidate();

    void Refresh(UserSession userSession);

    Task<UserSession> WaitForAuthenticationAsync(CancellationToken cancellationToken);
}