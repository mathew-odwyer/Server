// <copyright file="UserAccountEntityTypeConfiguration.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Mapping.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Domain.Entities.Users;

/// <summary>
/// Configures the <see cref="UserAccount"/> entity.
/// </summary>
public sealed class UserAccountEntityTypeConfiguration : IEntityTypeConfiguration<UserAccount>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .HasMany(u => u.Players)
            .WithOne()
            .HasForeignKey(p => p.UserAccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
