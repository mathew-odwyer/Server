using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work.Rooms;
using Winterhaven.API.Core.Domain.Entities.Rooms;

namespace Winterhaven.API.Infrastructure.Work.Rooms;

[ExcludeFromCodeCoverage]
internal sealed class RoomRepository : RepositoryBase<Room>, IRoomRepository
{
    public RoomRepository(DbContext context)
        : base(context)
    {
    }
}
