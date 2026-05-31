using System;
using System.Threading;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Core.Application.Services.Users;

/// <summary>
///   Defines an interface that provides access to the current user session.
/// </summary>
public interface IUserSessionContext : IDisposable
{
    /// <summary>
    ///   Gets a value indicating whether the current user session is authenticated.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a user session is currently authenticated; otherwise, <c>false</c>.
    /// </value>
    public bool IsAuthenticated { get; }

    /// <summary>
    ///   Gets a cancellation token that is cancelled when the current user session expires or is invalidated.
    /// </summary>
    /// <value>
    ///   A cancellation token associated with the lifetime of the current user session.
    /// </value>
    public CancellationToken SessionExpiredToken { get; }

    /// <summary>
    ///   Gets the user session (or <c>null</c> if one is not provided).
    /// </summary>
    /// <value>
    ///   The user session (or <c>null</c> if one is not provided).
    /// </value>
    public UserSession? UserSession { get; }
}
