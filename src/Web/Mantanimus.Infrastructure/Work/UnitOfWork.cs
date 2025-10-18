// <copyright file="UnitOfWork.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Work;

using Mantanimus.Core.Application.Exceptions.Database;
using Mantanimus.Core.Application.Work;
using Microsoft.EntityFrameworkCore;

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
            throw new DatabaseUpdateException("A database update exception occurred while saving changes.", ex);
        }
    }
}
