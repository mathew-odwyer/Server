using System;
using System.Threading;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

/// <summary>
/// </summary>
public interface IUserSessionExpiryNotifier : IDisposable
{
    /// <summary>
    /// </summary>
    public CancellationToken SessionExpiredToken { get; }
}
