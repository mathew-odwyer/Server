using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Domain.Entities;

namespace Winterhaven.API.Infrastructure.Interceptors;

/// <summary>
///   Intercepts save changes operations to update auditing fields for entities that inherit from <see cref="AuditableEntityBase"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    ///   Initializes a new instance of the <see cref="AuditableEntitySaveChangesInterceptor"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    ///   Specifies a <see cref="IServiceProvider"/> that is used to resolve the <see cref="IActorContext"/> upon updating entities.
    /// </param>
    public AuditableEntitySaveChangesInterceptor(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

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
    ///   Checks if any owned entities have changed in the given <see cref="EntityEntry"/>.
    /// </summary>
    /// <param name="entry">
    ///   Specifies an <see cref="EntityEntry"/> to check for changes in owned entities.
    /// </param>
    /// <returns>
    ///   Returns <c>true</c> if any owned entities have changed; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasChangedOwnedEntities(EntityEntry entry) => entry.References.Any(x => x.TargetEntry?.Metadata.IsOwned() == true && (x.TargetEntry.State == EntityState.Added || x.TargetEntry.State == EntityState.Modified));

    /// <summary>
    ///   Updates the auditing fields of entities in the given <see cref="DbContext"/>.
    /// </summary>
    /// <param name="context">
    ///   Specifies a <see cref="DbContext"/> whose entities are being audited.
    /// </param>
    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        // Resolve at save time, not construction time, to break the circular dependency.
        var actorContext = serviceProvider.GetRequiredService<IActorContext>();
        var identifier = actorContext.Actor.Id;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = identifier;
                entry.Entity.CreatedOn = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || HasChangedOwnedEntities(entry))
            {
                entry.Entity.ModifiedBy = identifier;
                entry.Entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
