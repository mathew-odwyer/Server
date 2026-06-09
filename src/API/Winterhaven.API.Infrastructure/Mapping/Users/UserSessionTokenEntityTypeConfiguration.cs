using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Infrastructure.Mapping.Users;

[ExcludeFromCodeCoverage]
internal sealed class UserSessionTokenEntityTypeConfiguration : AuditableEntityTypeConfigurationBase<UserSessionToken>
{
    public override void Configure(EntityTypeBuilder<UserSessionToken> builder)
    {
        base.Configure(builder);

        // Unique hashed refresh token (prevents reuse/duplication)
        builder
            .HasIndex(x => x.HashedRefreshToken)
            .IsUnique();

        builder
            .Property(x => x.AccessTokenExpirationDate)
            .IsRequired();

        builder
            .Property(x => x.HashedRefreshToken)
            .IsRequired()
            .HasMaxLength(44)
            .IsFixedLength();

        // This tells EF Core to include RowVersion in the WHERE clause when updating. If two processes try to expire the same token at the same time the first succeeds.
        builder
            .Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder
            .Property<Guid>("UserAccountId")
            .IsRequired();

        builder
            .HasOne(x => x.UserAccount)
            .WithMany()
            .HasForeignKey("UserAccountId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
