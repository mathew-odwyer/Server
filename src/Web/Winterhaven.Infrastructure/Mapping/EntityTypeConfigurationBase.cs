// <copyright file="EntityTypeConfigurationBase.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Mapping;

using System;
using Winterhaven.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Provides configuration for entities that inherit from <see cref="EntityBase"/>.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity being configured, which must inherit from <see cref="EntityBase"/>.
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
