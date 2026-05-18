namespace Winterhaven.API.Infrastructure.Work.Players;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Application.Work.Players;
using Winterhaven.API.Core.Domain.Entities.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(DbContext context)
        : base(context)
    {
    }
}