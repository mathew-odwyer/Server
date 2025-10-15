// <copyright file="UserClientTokenEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Mapping.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Domain.Entities.Users;

internal sealed class UserClientTokenEntityTypeConfiguration : AuditableEntityTypeConfigurationBase<UserClientToken>
{
    public override void Configure(EntityTypeBuilder<UserClientToken> builder)
    {
        base.Configure(builder);

        // Prevent duplicates and ensure fast lookups.
        builder
            .HasIndex(x => x.HashedToken)
            .IsUnique();

        builder
            .Property(e => e.ExpirationDate)
            .IsRequired();

        builder
            .Property(x => x.IsRevoked)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .Property(x => x.HashedToken)
            .IsRequired()
            .HasMaxLength(44)
            .IsFixedLength();

        // This tells EF Core to include RowVersion in the WHERE clause when updating.
        // If two processes try to expire the same token at the same time the first succeeds.
        builder
            .Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder
            .HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(x => x.UserAccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
