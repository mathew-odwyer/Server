namespace Winterhaven.API.Infrastructure.Mapping;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Entities;

/// <summary>
///   Provides configuration for entities that inherit from <see cref="EntityBase"/>.
/// </summary>
/// <typeparam name="TEntity">
///   The type of the entity being configured, which must inherit from <see cref="EntityBase"/>.
/// </typeparam>
/// <seealso cref="IEntityTypeConfiguration{TEntity}"/>
[ExcludeFromCodeCoverage]
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