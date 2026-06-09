using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Infrastructure.Mapping.Users;

[ExcludeFromCodeCoverage]
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
