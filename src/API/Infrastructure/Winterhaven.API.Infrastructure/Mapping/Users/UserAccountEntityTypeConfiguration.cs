namespace Winterhaven.API.Infrastructure.Mapping.Users;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
/// Configures the <see cref="UserAccount"/> entity.
/// </summary>
/// <see cref="AuditableEntityTypeConfigurationBase{TEntity}"/>
[ExcludeFromCodeCoverage]
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

        builder
            .HasOne(x => x.Player)
            .WithOne()
            .HasForeignKey<UserAccount>("PlayerId");

        base.Configure(builder);
    }
}