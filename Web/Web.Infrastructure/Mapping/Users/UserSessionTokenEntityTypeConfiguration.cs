// <copyright file="UserSessionTokenEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Mapping.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Domain.Entities.Users;

public sealed class UserSessionTokenEntityTypeConfiguration : AuditableEntityTypeConfigurationBase<UserSessionToken>
{
    public override void Configure(EntityTypeBuilder<UserSessionToken> builder)
    {
        base.Configure(builder);

        // Unique hashed refresh token (prevents reuse/duplication)
        builder
            .HasIndex(x => x.HashedRefreshToken)
            .IsUnique();

        builder
            .HasIndex(x => x.SessionId)
            .IsUnique();

        builder
            .Property(x => x.ExpirationDate)
            .IsRequired();

        builder
            .Property(x => x.HashedRefreshToken)
            .IsRequired()
            .HasMaxLength(44)
            .IsFixedLength();

        // This tells EF Core to include RowVersion in the WHERE clause when updating.
        // If two processes try to expire the same token at the same time the first succeeds.
        builder
            .Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder
            .HasOne(x => x.UserAccount)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
