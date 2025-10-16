// <copyright file="IUserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Users;

using Web.Domain.Entities.Users;

public interface IUserAccountRepository
{
    UserAccount? GetById(Guid id, CancellationToken cancellationToken = default);
}
