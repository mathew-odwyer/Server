using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Domain.Entities.Rooms;

namespace Winterhaven.API.Infrastructure.Services.Seeds.Rooms;

internal sealed class RoomSeedService
{
    public static void Seed(DbContext databaseContext)
    {
        ArgumentNullException.ThrowIfNull(databaseContext);

        var rooms = new List<Room>
        {
            new() { MapName = "Bellmare Tavern", MapFilePath = "Bellmare/tavern" }
        };

        //// Remove all existing rooms before seeding the data, just to be safe.
        databaseContext.Set<Room>().RemoveRange(databaseContext.Set<Room>());

        // Seed the rooms
        databaseContext.Set<Room>().AddRange(rooms);

        databaseContext.SaveChanges();
    }
}
