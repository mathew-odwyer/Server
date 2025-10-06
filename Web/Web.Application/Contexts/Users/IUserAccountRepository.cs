// <copyright file="IUserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts.Users;

using Web.Domain.Entities.Users;

/// <summary>
/// Defines an interface that represents a <see cref="UserAccount"/> repository.
/// </summary>
public interface IUserAccountRepository
{
    /// <summary>
    /// Gets the <see cref="UserAccount"/> associated with the specified <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">
    /// The identifier of the <see cref="UserAccount"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Returns the <see cref="UserAccount"/> associated with the specified <paramref name="identifier"/>; otherwise, <c>null</c>.
    /// </returns>
    Task<UserAccount?> GetByIdAsync(string identifier, CancellationToken cancellationToken = default);
}
