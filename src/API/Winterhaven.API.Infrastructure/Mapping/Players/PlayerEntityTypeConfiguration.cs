using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities.Players;

namespace Winterhaven.API.Infrastructure.Mapping.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerEntityTypeConfiguration : EntityTypeConfigurationBase<Player>
{
    public override void Configure(EntityTypeBuilder<Player> builder)
    {
        builder
            .HasIndex(p => p.Name)
            .IsUnique();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(12);

        builder
            .Property(x => x.X)
            .IsRequired()
            .HasDefaultValue(128);

        builder
            .Property(x => x.Y)
            .IsRequired()
            .HasDefaultValue(128);

        base.Configure(builder);
    }
}
