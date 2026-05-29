using System;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Core.Application.Work.Users;

/// <summary>
/// </summary>
public interface IActorRepository : IRepository<Actor>
{
    /// <summary>
    /// </summary>
    /// <param name="id">
    /// </param>
    /// <returns>
    /// </returns>
    public Actor? GetById(Guid id);
}