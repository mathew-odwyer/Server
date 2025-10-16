// <copyright file="UserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Work.Users;

using Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Defines an interface that represents a repository for managing <see cref="UserAccount"/> entities.
/// </summary>
/// <seealso cref="IRepository{TEntity}"/>
public interface IUserAccountRepository : IRepository<UserAccount>
{
    /// <summary>
    /// Gets a <see cref="UserAccount"/> by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// Specifies a <see cref="Guid"/> that represents the unique identifier of the user account.
    /// </param>
    /// <returns>
    /// Returns a <see cref="UserAccount"/> if found; otherwise, <c>null</c>.
    /// </returns>
    UserAccount? GetById(Guid id);
}
