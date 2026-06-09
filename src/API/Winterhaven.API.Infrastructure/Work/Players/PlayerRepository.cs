using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work.Players;
using Winterhaven.API.Core.Domain.Entities.Players;

namespace Winterhaven.API.Infrastructure.Work.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
{
    public PlayerRepository(DbContext context)
        : base(context)
    {
    }
}
