using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Infrastructure.Mapping.Users;

[ExcludeFromCodeCoverage]
internal sealed class UserAccountEntityTypeConfiguration : AuditableEntityTypeConfigurationBase<UserAccount>
{
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
            .HasForeignKey<Player>(x => x.Id);

        base.Configure(builder);
    }
}
