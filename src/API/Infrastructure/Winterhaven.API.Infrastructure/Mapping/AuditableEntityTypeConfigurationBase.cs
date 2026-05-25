namespace Winterhaven.API.Infrastructure.Mapping;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Entities;

/// <summary>
///   Provides configuration for entities that inherit from <see cref="AuditableEntityBase"/>.
/// </summary>
/// <typeparam name="TEntity">
///   The type of the entity being configured, which must inherit from <see cref="AuditableEntityBase"/>.
/// </typeparam>
/// <seealso cref="EntityTypeConfigurationBase{TEntity}"/>
[ExcludeFromCodeCoverage]
public abstract class AuditableEntityTypeConfigurationBase<TEntity> : EntityTypeConfigurationBase<TEntity>
    where TEntity : AuditableEntityBase
{
    /// <inheritdoc/>
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