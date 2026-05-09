namespace Winterhaven.API.Infrastructure.Work.Users;

using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

[ExcludeFromCodeCoverage]
internal sealed class ActorRepository : Repository<Actor>, IActorRepository
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