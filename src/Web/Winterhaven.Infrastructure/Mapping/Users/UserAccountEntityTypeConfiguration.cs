// <copyright file="UserAccountEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Mapping.Users;

using Winterhaven.Core.Domain.Entities.Players;
using Winterhaven.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Configures the <see cref="UserAccount"/> entity.
/// </summary>
/// <see cref="AuditableEntityTypeConfigurationBase{TEntity}"/>
public sealed class UserAccountEntityTypeConfiguration : AuditableEntityTypeConfigurationBase<UserAccount>
{
    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .HasIndex(x => x.Username)
            .IsUnique();

        builder
            .HasIndex(x => x.EmailAddress)
            .IsUnique();

        builder
            .Property(x => x.Username)
            .HasMaxLength(12)
            .IsRequired();

        builder
            .Property(x => x.EmailAddress)
            .HasMaxLength(320)
            .IsRequired();

        // Shared primary key one-to-one relationship
        builder
            .HasOne(x => x.Player)
            .WithOne()
            .HasForeignKey<Player>(x => x.Id);

        base.Configure(builder);
    }
}
