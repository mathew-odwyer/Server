// <copyright file="IUserAccountService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Services.Users;

using FluentResults;
using Web.Domain.Entities.Users;

/// <summary>
/// Provides user account management operations, such as registration.
/// </summary>
public interface IUserAccountService
{
    Task<Result<UserAccount>> LoginUserAsync(string username, string password);

    /// <summary>
    /// Registers a new user account with the specified credentials.
    /// </summary>
    /// <param name="emailAddress">
    /// The email address to associate with the new user account.
    /// </param>
    /// <param name="username">
    /// The username for the new user account.
    /// </param>
    /// <param name="password">
    /// The password for the new user account.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// The result contains the created <see cref="UserAccount"/> on success.
    /// On failure, the result contains one or more errors describing why registration did not succeed,
    /// such as duplicate email/username, invalid input, or password policy violations.
    /// </returns>
    Task<Result<UserAccount>> RegisterUserAsync(string emailAddress, string username, string password);
}
