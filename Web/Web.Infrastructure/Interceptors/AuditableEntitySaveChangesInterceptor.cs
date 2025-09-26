// <copyright file="AuditableEntitySaveChangesInterceptor.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Interceptors;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Web.Domain.Entities;

/// <summary>
/// Intercepts save changes operations to update auditing fields for entities that inherit from <see cref="AuditableEntityBase"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Checks if any owned entities have changed in the given <see cref="EntityEntry"/>.
    /// </summary>
    /// <param name="entry">
    /// Specifies an <see cref="EntityEntry"/> to check for changes in owned entities.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if any owned entities have changed; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasChangedOwnedEntities(EntityEntry entry)
    {
        return entry.References.Any(x => x.TargetEntry != null && x.TargetEntry.Metadata.IsOwned() && (x.TargetEntry.State == EntityState.Added || x.TargetEntry.State == EntityState.Modified));
    }

    /// <summary>
    /// Updates the auditing fields of entities in the given <see cref="DbContext"/>.
    /// </summary>
    /// <param name="context">
    /// Specifies a <see cref="DbContext"/> whose entities are being audited.
    /// </param>
    private static void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = Guid.Parse("E8A05ABB-C67C-4FC0-88EB-3553E265161A");
                entry.Entity.CreatedOn = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
            {
                entry.Entity.ModifiedBy = Guid.Parse("E8A05ABB-C67C-4FC0-88EB-3553E265161A");
                entry.Entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
