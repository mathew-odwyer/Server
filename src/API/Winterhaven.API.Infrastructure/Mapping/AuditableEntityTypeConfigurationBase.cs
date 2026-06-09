using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities;

namespace Winterhaven.API.Infrastructure.Mapping;

[ExcludeFromCodeCoverage]
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
