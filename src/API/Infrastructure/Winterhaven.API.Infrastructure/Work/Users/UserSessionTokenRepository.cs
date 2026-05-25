namespace Winterhaven.API.Infrastructure.Work.Users;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

internal sealed class UserSessionTokenRepository : RepositoryBase<UserSessionToken>, IUserSessionTokenRepository
{
    public UserSessionTokenRepository(DbContext context)
        : base(context)
    {
    }

    public async Task<UserSessionToken?> GetActiveSessionAsync(Guid userAccountId, CancellationToken cancellationToken = default)
    {
        return await this.Query()
            .Where(x =>
                x.UserAccount.Id == userAccountId &&
                x.AccessTokenExpirationDate > DateTime.UtcNow &&
                !x.IsRevoked)
            .OrderByDescending(x => x.AccessTokenExpirationDate)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}