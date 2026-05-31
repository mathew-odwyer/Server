using System;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Core.Application.Services.Users;

/// <summary>
///   Defines an interface that provides access to the current user session.
/// </summary>
public interface IUserSessionContext : IDisposable
{
    /// <summary>
    ///   Occurs when we the user session has been established.
    /// </summary>
    public event EventHandler<EventArgs>? Established;

    /// <summary>
    ///   Occurs when the user session has been invalidated.
    /// </summary>
    public event EventHandler<EventArgs>? Invalidated;

    /// <summary>
    ///   Occurs when the user session has been refreshed.
    /// </summary>
    public event EventHandler<EventArgs>? Refreshed;

    /// <summary>
    ///   Gets a value indicating whether the current user session is authenticated.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a user session is currently authenticated; otherwise, <c>false</c>.
    /// </value>
    public bool IsAuthenticated { get; }

    /// <summary>
    ///   Gets the user session (or <c>null</c> if one is not provided).
    /// </summary>
    /// <value>
    ///   The user session (or <c>null</c> if one is not provided).
    /// </value>
    public UserSession? UserSession { get; }
}
