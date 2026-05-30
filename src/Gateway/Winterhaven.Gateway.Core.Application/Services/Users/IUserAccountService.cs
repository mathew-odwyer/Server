using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Gateway.Core.Application.Services.Users;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record UserLoginResult(
    string RefreshToken);

/// <summary>
///   Defines an interface that provides functions to handle user accounts (registration, login, etc).
/// </summary>
public interface IUserAccountService
{
    /// <summary>
    /// </summary>
    public Task<UserLoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    ///   Registers a potential user.
    /// </summary>
    /// <param name="username">
    ///   The username of the potential user to be registered.
    /// </param>
    /// <param name="password">
    ///   The password of the potential user to be registered.
    /// </param>
    /// <param name="emailAddress">
    ///   The email address of the potential user to be registered.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token used to cancel the registration.
    /// </param>
    /// <returns>
    ///   Returns a task that completes when the registration succeeds.
    /// </returns>
    public Task RegisterAsync(string username, string password, string emailAddress, CancellationToken cancellationToken = default);
}
