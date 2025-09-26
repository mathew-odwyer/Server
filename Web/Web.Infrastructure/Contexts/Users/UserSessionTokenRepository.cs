// <copyright file="UserSessionTokenRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts.Users;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts.Users;
using Web.Domain.Entities.Users;

internal sealed class UserSessionTokenRepository : Repository<UserSessionToken>, IUserSessionTokenRepository
{
    public UserSessionTokenRepository(DbContext context)
        : base(context)
    {
    }

    public async Task<UserSessionToken?> GetActiveSessionAsync(string userAccountId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userAccountId);

        return await this.Query()
            .Where(x =>
                x.UserAccount.Id == userAccountId &&
                x.ExpirationDate > DateTime.UtcNow)
            .Include(x => x.UserAccount)
            .OrderByDescending(x => x.ExpirationDate)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<UserSessionToken?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await this.Query()
            .Where(x =>
                x.SessionId == sessionId &&
                x.ExpirationDate > DateTime.UtcNow)
            .OrderByDescending(x => x.ExpirationDate)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
