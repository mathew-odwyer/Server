// <copyright file="PlayerRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts.Players;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts.Players;
using Web.Domain.Entities.Players;

internal sealed class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(DbContext context)
        : base(context)
    {
    }

    public async Task<Player?> GetPlayerByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await this.Query()
            .FirstOrDefaultAsync(x => x.NormalizedName == name.ToUpperInvariant(), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> IsPlayerExists(string name, CancellationToken cancellationToken = default)
    {
        return await this.Query()
            .AnyAsync(x => x.NormalizedName == name.ToUpperInvariant(), cancellationToken)
            .ConfigureAwait(false);
    }
}
