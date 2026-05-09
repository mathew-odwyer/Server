namespace Winterhaven.API.Core.Application.Work.Users;

using System;
using Winterhaven.API.Core.Domain.Entities.Users;

public interface IActorRepository : IRepository<Actor>
{
    Actor? GetById(Guid id);
}