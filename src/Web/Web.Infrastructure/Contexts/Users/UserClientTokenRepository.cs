// <copyright file="UserClientTokenRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts.Users;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts.Users;
using Web.Domain.Entities.Users;

internal sealed class UserClientTokenRepository : Repository<UserClientToken>, IUserClientTokenRepository
{
    public UserClientTokenRepository(DbContext context)
        : base(context)
    {
    }

    public async Task<UserClientToken?> GetByHashedToken(string hashedToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedToken);

        return await this.Query()
             .Where(x =>
                 x.HashedToken == hashedToken &&
                 x.ExpirationDate > DateTime.UtcNow &&
                 !x.IsRevoked)
             .OrderByDescending(x => x.ExpirationDate)
             .FirstOrDefaultAsync(cancellationToken)
             .ConfigureAwait(false);
    }
}
