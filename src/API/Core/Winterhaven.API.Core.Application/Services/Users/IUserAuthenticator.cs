namespace Winterhaven.API.Core.Application.Services.Users;

using System.Threading.Tasks;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
/// Defines an interface that provides functionality for authenticating a <see cref="UserAccount"/>.
/// </summary>
public interface IUserAuthenticator
{
    /// <summary>
    /// Authenticates an existing user using the provided <paramref name="username"/> and <paramref name="password"/>.
    /// </summary>
    /// <param name="username">
    /// Specifies a <see cref="string"/> representing the username of the user attempting to log in.
    /// </param>
    /// <param name="password">
    /// Specifies a <see cref="string"/> representing the password associated with the provided username.
    /// </param>
    /// <returns>
    /// Returns a <see cref="Task{TResult}"/> representing the asynchronous operation. The task
    /// result contains a <see cref="UserAccount"/> representing the authenticated user if login succeeds.
    /// </returns>
    Task<UserAccount> LoginUserAsync(string username, string password);
}