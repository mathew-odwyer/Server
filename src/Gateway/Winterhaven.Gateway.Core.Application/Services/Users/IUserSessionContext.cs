using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Core.Application.Services.Users;

/// <summary>
///   Defines an interface that provides access to the current user session.
/// </summary>
public interface IUserSessionContext
{
    /// <summary>
    ///   Gets the user session (or <c>null</c> if one is not provided).
    /// </summary>
    /// <value>
    ///   The user session (or <c>null</c> if one is not provided).
    /// </value>
    public UserSession? UserSession { get; }
}
