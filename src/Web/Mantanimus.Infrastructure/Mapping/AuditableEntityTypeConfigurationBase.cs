// <copyright file="AuditableEntityTypeConfigurationBase.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Mapping;

using Mantanimus.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal abstract class AuditableEntityTypeConfigurationBase<TEntity> : EntityTypeConfigurationBase<TEntity>
    where TEntity : AuditableEntityBase
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Property(nameof(AuditableEntityBase.CreatedOn))
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired(true);

        builder
            .Property(nameof(AuditableEntityBase.CreatedBy))
            .HasDefaultValue(Guid.Empty)
            .IsRequired(true);

        builder
            .Property(nameof(AuditableEntityBase.ModifiedOn))
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired(true);

        builder
            .Property(nameof(AuditableEntityBase.ModifiedBy))
            .HasDefaultValue(Guid.Empty)
            .IsRequired(true);

        base.Configure(builder);
    }
}
