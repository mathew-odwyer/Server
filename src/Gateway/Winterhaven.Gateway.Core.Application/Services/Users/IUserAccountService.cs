using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Gateway.Core.Application.Services.Users;

/// <summary>
///   Represents a user registration result.
/// </summary>
/// <param name="Success">
///   Indicates whether the user has been registered.
/// </param>
public sealed record UserRegistrationResult(
    bool Success);

/// <summary>
///   Defines an interface that provides functions to handle user accounts (registration, login, etc).
/// </summary>
public interface IUserAccountService
{
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
    ///   Returns a result that indicates whether the registration was successful.
    /// </returns>
    public Task<UserRegistrationResult> RegisterAsync(string username, string password, string emailAddress, CancellationToken cancellationToken = default);
}
