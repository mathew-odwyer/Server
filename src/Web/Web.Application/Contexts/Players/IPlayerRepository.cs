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
}
