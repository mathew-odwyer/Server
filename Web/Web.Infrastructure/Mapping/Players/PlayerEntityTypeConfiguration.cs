// <copyright file="PlayerEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Mapping.Players;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Domain.Entities.Players;

/// <summary>
/// Configures the <see cref="Player"/> entity.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class PlayerEntityTypeConfiguration : AuditableEntityTypeConfigurationBase<Player>
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<Player> builder)
    {
        builder
            .Property(p => p.Name)
            .IsRequired();

        builder
            .HasIndex(p => p.Name)
            .IsUnique();

        base.Configure(builder);
    }
}
