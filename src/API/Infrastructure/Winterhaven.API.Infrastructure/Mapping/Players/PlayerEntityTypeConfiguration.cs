namespace Winterhaven.API.Infrastructure.Mapping.Players;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
///   Configures the <see cref="UserSessionToken"/> entity.
/// </summary>
/// <seealso cref="AuditableEntityTypeConfigurationBase{TEntity}"/>
[ExcludeFromCodeCoverage]
public sealed class PlayerEntityTypeConfiguration : EntityTypeConfigurationBase<Player>
{
    /// <inheritdoc/>
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