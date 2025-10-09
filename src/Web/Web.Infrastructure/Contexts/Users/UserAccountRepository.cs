// <copyright file="UserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts.Users;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts.Users;
using Web.Domain.Entities.Users;

internal sealed class UserAccountRepository : IUserAccountRepository
{
    private readonly DbContext context;

    public UserAccountRepository(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<UserAccount?> GetByIdAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await this.context.Set<UserAccount>()
            .FirstOrDefaultAsync(x => x.Id == identifier, cancellationToken)
            .ConfigureAwait(false);
    }
}
