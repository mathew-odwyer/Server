using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

/// <summary>
///   Defines an interface for authenticating and managing user sessions.
/// </summary>
public interface IUserSessionAuthenticator
{
    /// <summary>
    ///   Gets a value indicating whether the current user session is authenticated.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a user session is currently authenticated; otherwise, <c>false</c>.
    /// </value>
    public bool IsAuthenticated { get; }

    /// <summary>
    ///   Authenticates a user session.
    /// </summary>
    /// <param name="userSession">
    ///   The <see cref="UserSession"/> instance containing the user credentials to authenticate.
    /// </param>
    /// <remarks>
    ///   After calling this method, <see cref="IsAuthenticated"/> should return <c>true</c>.
    /// </remarks>
    public void Authenticate(UserSession userSession);

    /// <summary>
    ///   Invalidates the current authenticated session.
    /// </summary>
    /// <remarks>
    ///   After calling this method, <see cref="IsAuthenticated"/> should return <c>false</c>.
    /// </remarks>
    public void Invalidate();

    /// <summary>
    ///   Refreshes the current authenticated session with updated credentials.
    /// </summary>
    /// <param name="userSession">
    ///   The <see cref="UserSession"/> instance containing updated tokens and expiration.
    /// </param>
    /// <remarks>
    ///   Use this method to extend or update the session without requiring a full re-authentication.
    /// </remarks>
    public void Refresh(UserSession userSession);
}
