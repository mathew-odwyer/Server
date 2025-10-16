// <copyright file="IUserSessionTokenRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Users;

using Web.Domain.Entities.Users;

/// <summary>
/// Defines an interface that represents a <see cref="UserSessionToken"/> repository.
/// </summary>
/// <seealso cref="IRepository{TEntity}"/>
public interface IUserSessionTokenRepository : IRepository<UserSessionToken>
{
    /// <summary>
    /// Gets the currently active session for the <see cref="UserAccount"/> that matches the specified <paramref name="userAccountId"/>.
    /// </summary>
    /// <param name="userAccountId">
    /// The user account identifier of the <see cref="UserAccount"/> to fetch the active session for.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Returns the currently active session for the <see cref="UserAccount"/> that matches the specified <paramref name="userAccountId"/>; otherwise, <c>null</c>.
    /// </returns>
    Task<UserSessionToken?> GetActiveSessionAsync(Guid userAccountId, CancellationToken cancellationToken = default);
}
