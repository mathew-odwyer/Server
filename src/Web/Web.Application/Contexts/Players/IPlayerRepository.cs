// <copyright file="IPlayerRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Players;

using Web.Domain.Entities.Players;

/// <summary>
/// Defines an interface that represents a <see cref="Player"/> repository.
/// </summary>
/// <seealso cref="IRepository{TEntity}"/>
public interface IPlayerRepository : IRepository<Player>
{
    /// <summary>
    /// Gets a <see cref="Player"/> that matches the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// The name of the <see cref="Player"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Returns the <see cref="Player"/> that matches the specified <paramref name="name"/>; otherwise, <c>null</c>.
    /// </returns>
    Task<Player?> GetPlayerByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all players associated with the specified <paramref name="userAccountId"/>.
    /// </summary>
    /// <param name="userAccountId">
    /// The user account identifier used to fetch all players.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Returns all players associated with the specified <paramref name="userAccountId"/>.
    /// </returns>
    Task<IEnumerable<Player>> GetPlayersByUserAccountId(string userAccountId, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a player with the specified <paramref name="name"/> exists.
    /// </summary>
    /// <param name="name">
    /// The name of the <see cref="Player"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if a player with the specified <paramref name="name"/> exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsPlayerExists(string name, CancellationToken cancellationToken = default);
}
