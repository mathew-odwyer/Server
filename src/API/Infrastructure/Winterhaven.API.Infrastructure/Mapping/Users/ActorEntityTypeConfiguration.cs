namespace Winterhaven.API.Infrastructure.Mapping.Users;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Winterhaven.API.Core.Domain.Entities.Users;

internal sealed class ActorEntityTypeConfiguration : EntityTypeConfigurationBase<Actor>
{
    public override void Configure(EntityTypeBuilder<Actor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .HasIndex(x => x.Name)
            .IsUnique();

        builder
            .Property(x => x.Name)
            .HasMaxLength(12)
            .IsRequired();
    }
}