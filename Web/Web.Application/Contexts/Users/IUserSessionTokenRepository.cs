// <copyright file="IUserSessionTokenRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Users;

using Web.Domain.Entities.Users;

public interface IUserSessionTokenRepository : IRepository<UserSessionToken>
{
    Task<UserSessionToken?> GetActiveSessionAsync(string userAccountId, CancellationToken cancellationToken = default);

    Task<UserSessionToken?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
