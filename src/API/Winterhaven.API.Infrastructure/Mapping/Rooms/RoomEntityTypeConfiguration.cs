using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities.Rooms;

namespace Winterhaven.API.Infrastructure.Mapping.Rooms;

[ExcludeFromCodeCoverage]
internal sealed class RoomEntityTypeConfiguration : EntityTypeConfigurationBase<Room>
{
    public override void Configure(EntityTypeBuilder<Room> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // All map names are unique at the DB level.
        builder
            .HasIndex(r => r.MapName)
            .IsUnique();

        builder
            .Property(x => x.MapName)
            .IsRequired();

        builder
            .Property(x => x.MapFilePath)
            .IsRequired();

        base.Configure(builder);
    }
}
