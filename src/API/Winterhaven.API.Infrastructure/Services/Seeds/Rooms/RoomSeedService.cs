using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Rooms;
using Winterhaven.API.Core.Domain.Entities.Rooms;

namespace Winterhaven.API.Infrastructure.Services.Seeds.Rooms;

internal static class RoomSeedService
{
    public static async Task SeedAsync(IUnitOfWorkFactory unitOfWorkFactory, IRoomRepository roomRepository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkFactory);
        ArgumentNullException.ThrowIfNull(roomRepository);

        var rooms = new List<Room>
        {
            new() { MapName = "Bellmare Tavern", MapFilePath = "Bellmare/tavern" }
        };

        var work = unitOfWorkFactory.CreateUnitOfWork();

        //// Remove all existing rooms before seeding the data, just to be safe.
        roomRepository.DeleteAll();

        // See the rooms.
        await roomRepository.AddRangeAsync(rooms).ConfigureAwait(false);
        await work.SaveAsync().ConfigureAwait(false);
    }
}
