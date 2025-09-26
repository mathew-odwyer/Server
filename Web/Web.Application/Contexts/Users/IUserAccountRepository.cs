// <copyright file="IUserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Users;

using Web.Domain.Entities.Users;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByIdAsync(string identifier, CancellationToken cancellationToken = default);
}
