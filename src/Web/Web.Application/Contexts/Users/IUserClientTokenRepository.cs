// <copyright file="IUserClientTokenRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Users;

using Web.Domain.Entities.Users;

public interface IUserClientTokenRepository : IRepository<UserClientToken>
{
    Task<UserClientToken?> GetByHashedToken(string hashedToken, CancellationToken cancellationToken = default);
}
