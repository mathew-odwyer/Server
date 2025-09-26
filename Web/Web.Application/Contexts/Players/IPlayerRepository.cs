// <copyright file="IPlayerRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Players;

using Web.Domain.Entities.Players;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetPlayerByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> IsPlayerExists(string name, CancellationToken cancellationToken = default);
}
