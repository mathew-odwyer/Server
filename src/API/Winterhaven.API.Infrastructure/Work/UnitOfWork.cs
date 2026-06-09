using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Infrastructure.Work;

[ExcludeFromCodeCoverage]
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbContext context;

    public UnitOfWork(DbContext context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            throw new EntityPersistenceException("A database update exception occurred while saving changes.", ex);
        }
    }
}
