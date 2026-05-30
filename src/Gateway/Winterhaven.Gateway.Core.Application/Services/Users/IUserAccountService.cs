using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Gateway.Core.Application.Services.Users;

/// <summary>
///   Represents a user loggin result.
/// </summary>
/// <param name="RefreshToken">
///   The refresh token used to refresh the users session.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UserLoginResult(
    string RefreshToken);

/// <summary>
///   Defines an interface that provides functions to handle user accounts (registration, login, etc).
/// </summary>
public interface IUserAccountService
{
    /// <summary>
    ///   Authenticates and logs in a potential user.
    /// </summary>
    /// <param name="username">
    ///   The username of the potential user to be logged in.
    /// </param>
    /// <param name="password">
    ///   The password of the potential user to be logged in.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token used to cancel the login request.
    /// </param>
    /// <returns>
    ///   Returns a <see cref="UserLoginResult"/> that is the result of the login operation.
    /// </returns>
    public Task<UserLoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    ///   Logs out the currently authenticated user and invalidates their active session.
    /// </summary>
    /// <param name="cancellationToken">
    ///   The cancellation token used to cancel the logout request.
    /// </param>
    /// <returns>
    ///   Returns a task that completes when the logout operation succeeds.
    /// </returns>
    public Task LogoutAsync(CancellationToken cancellationToken = default);

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
