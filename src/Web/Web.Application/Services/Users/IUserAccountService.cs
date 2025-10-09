// <copyright file="IUserAccountService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Services.Users;

using Web.Domain.Entities.Users;

/// <summary>
/// Provides operations for managing user accounts, including registration and login functionality.
/// </summary>
public interface IUserAccountService
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
    /// Returns a <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a <see cref="UserAccount"/> representing the authenticated user if login succeeds.
    /// </returns>
    Task<UserAccount> LoginUserAsync(string username, string password);

    /// <summary>
    /// Registers a new user with the specified <paramref name="emailAddress"/>, <paramref name="username"/>, and <paramref name="password"/>.
    /// </summary>
    /// <param name="emailAddress">
    /// Specifies a <see cref="string"/> representing the email address of the user to register.
    /// The email address must not be empty and must be in a valid email format.
    /// </param>
    /// <param name="username">
    /// Specifies a <see cref="string"/> representing the desired username of the new user.
    /// The username must be between 3 and 12 characters and may only contain alphanumeric characters, hyphens ('-'), or underscores ('_').
    /// </param>
    /// <param name="password">
    /// Specifies a <see cref="string"/> representing the desired password for the new user.
    /// The password must be at least 12 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.
    /// </param>
    /// <returns>
    /// Returns a <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// The task result contains a <see cref="UserAccount"/> representing the newly registered user if registration succeeds.
    /// </returns>
    Task<UserAccount> RegisterUserAsync(string emailAddress, string username, string password);
}
