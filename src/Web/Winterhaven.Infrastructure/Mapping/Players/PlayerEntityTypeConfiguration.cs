// <copyright file="PlayerEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Mapping.Players;

using Winterhaven.Core.Domain.Entities.Players;
using Winterhaven.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Configures the <see cref="UserSessionToken"/> entity.
/// </summary>
/// <seealso cref="AuditableEntityTypeConfigurationBase{TEntity}"/>
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
