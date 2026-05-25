namespace Winterhaven.API.Core.Application.Services.Users;

using System.Threading.Tasks;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
///   Defines an interface for registering new users in the system.
/// </summary>
public interface IUserRegistrar
{
    /// <summary>
    ///   Registers a new user with the specified <paramref name="emailAddress"/>, <paramref name="username"/>, and <paramref name="password"/>.
    /// </summary>
    /// <param name="emailAddress">
    ///   Specifies a <see cref="string"/> representing the email address of the user to register. The email address must not be empty and must be in a valid email format.
    /// </param>
    /// <param name="username">
    ///   Specifies a <see cref="string"/> representing the desired username of the new user. The username must be between 3 and 12 characters and may only contain alphanumeric characters, hyphens ('-'), or underscores ('_').
    /// </param>
    /// <param name="password">
    ///   Specifies a <see cref="string"/> representing the desired password for the new user. The password must be at least 12 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.
    /// </param>
    /// <returns>
    ///   Returns a <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains a user account representing the newly registered user if registration succeeds.
    /// </returns>
    Task<UserAccount> RegisterUserAsync(string emailAddress, string username, string password);
}