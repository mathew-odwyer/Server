using System;
using System.Threading;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

/// <summary>
///   Defines an interface that provides a means to be notified when user session expires.
/// </summary>
public interface IUserSessionExpiryNotifier : IDisposable
{
    /// <summary>
    ///   The cancellation token used to operate on expired sessions.
    /// </summary>
    public CancellationToken SessionExpiredToken { get; }
}
