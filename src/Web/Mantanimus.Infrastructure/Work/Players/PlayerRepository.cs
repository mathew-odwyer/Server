// <copyright file="PlayerRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Work.Players;

using Mantanimus.Core.Application.Work.Players;
using Mantanimus.Core.Domain.Entities.Players;
using Microsoft.EntityFrameworkCore;

internal sealed class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(DbContext context)
        : base(context)
    {
    }
}
