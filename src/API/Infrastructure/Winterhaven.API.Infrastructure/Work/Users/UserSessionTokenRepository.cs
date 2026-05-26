using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Infrastructure.Work.Users;

internal sealed class UserSessionTokenRepository : RepositoryBase<UserSessionToken>, IUserSessionTokenRepository
{
    public UserSessionTokenRepository(DbContext context)
        : base(context)
    {
    }

    public async Task<UserSessionToken?> GetActiveSessionAsync(Guid userAccountId, CancellationToken cancellationToken = default)
        => await Query()
            .Where(x =>
                x.UserAccount.Id == userAccountId &&
                x.AccessTokenExpirationDate > DateTime.UtcNow &&
                !x.IsRevoked)
            .OrderByDescending(x => x.AccessTokenExpirationDate)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
}