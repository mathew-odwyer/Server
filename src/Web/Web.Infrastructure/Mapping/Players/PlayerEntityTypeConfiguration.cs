// <copyright file="PlayerEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Mapping.Players;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

internal sealed class PlayerEntityTypeConfiguration : EntityTypeConfigurationBase<Player>
{
    public override void Configure(EntityTypeBuilder<Player> builder)
    {
        builder
           .HasOne<UserAccount>()
           .WithOne(u => u.Player)
           .HasForeignKey<Player>(p => p.Id)
           .IsRequired();

        builder
            .HasIndex(p => p.Name)
            .IsUnique();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(12);

        builder
            .Property(x => x.X)
            .IsRequired();

        builder
            .Property(x => x.Y)
            .IsRequired();

        base.Configure(builder);
    }
}
