// <copyright file="PlayerRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts.Players;

using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts.Players;
using Web.Domain.Entities.Players;

internal sealed class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(DbContext context)
        : base(context)
    {
    }
}
