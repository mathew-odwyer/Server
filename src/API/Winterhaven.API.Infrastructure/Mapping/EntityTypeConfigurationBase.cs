using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities;

namespace Winterhaven.API.Infrastructure.Mapping;

/// <summary>
///   Provides configuration for entities that inherit from <see cref="EntityBase"/>.
/// </summary>
/// <typeparam name="TEntity">
///   The type of the entity being configured, which must inherit from <see cref="EntityBase"/>.
/// </typeparam>
/// <seealso cref="IEntityTypeConfiguration{TEntity}"/>
public abstract class EntityTypeConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase
{
    /// <inheritdoc/>
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .HasKey(x => x.Id);
    }
}