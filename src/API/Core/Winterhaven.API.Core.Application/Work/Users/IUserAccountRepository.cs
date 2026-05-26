using System;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Core.Application.Work.Users;

/// <summary>
///   Defines an interface that represents a repository for managing user account entities.
/// </summary>
public interface IUserAccountRepository : IRepository<UserAccount>
{
    /// <summary>
    ///   Gets a user account by its unique identifier.
    /// </summary>
    /// <param name="id">
    ///   Specifies a <see cref="Guid"/> that represents the unique identifier of the user account.
    /// </param>
    /// <returns>
    ///   Returns a user account if found; otherwise, <c>null</c>.
    /// </returns>
    public UserAccount? GetById(Guid id);
}