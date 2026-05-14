namespace Winterhaven.API.Infrastructure.Work;

using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.Common.Exceptions;

[ExcludeFromCodeCoverage]
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbContext? context;

    public UnitOfWork(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await this.context!.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            throw new EntityPersistenceException("A database update exception occurred while saving changes.", ex);
        }
    }
}