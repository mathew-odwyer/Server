namespace Winterhaven.API.Infrastructure.Work.Users;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

internal sealed class ActorRepository : RepositoryBase<Actor>, IActorRepository
{
    public ActorRepository(DbContext context)
        : base(context)
    {
    }

    public Actor? GetById(Guid id)
    {
        return this.Query().FirstOrDefault(x => x.Id == id);
    }
}