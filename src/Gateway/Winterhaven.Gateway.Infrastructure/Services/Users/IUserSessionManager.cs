using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

/// <summary>
///   Defines an interface for managing the lifecycle of the current user session.
/// </summary>
public interface IUserSessionManager
{
    /// <summary>
    ///   Establishes a new user session.
    /// </summary>
    /// <param name="userSession">
    ///   The user session to establish.
    /// </param>
    public void EstablishUserSession(UserSession userSession);

    /// <summary>
    ///   Invalidates the current user session.
    /// </summary>
    public void InvalidateUserSession();

    /// <summary>
    ///   Refreshes the current user session with updated session data.
    /// </summary>
    /// <param name="userSession">
    ///   The updated user session.
    /// </param>
    public void RefreshUserSession(UserSession userSession);
}
