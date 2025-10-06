// <copyright file="IUserAccountService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Services.Users;

using Web.Domain.Entities.Users;

/// <summary>
/// Provides user account management operations, such as registration.
/// </summary>
public interface IUserAccountService
{
    Task<UserAccount> LoginUserAsync(string username, string password);

    Task<UserAccount> RegisterUserAsync(string emailAddress, string username, string password);
}
