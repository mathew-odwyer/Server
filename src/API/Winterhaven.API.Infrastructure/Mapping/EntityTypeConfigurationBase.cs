using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Winterhaven.API.Core.Domain.Entities;

namespace Winterhaven.API.Infrastructure.Mapping;

[ExcludeFromCodeCoverage]
internal abstract class EntityTypeConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .HasKey(x => x.Id);
    }
}
