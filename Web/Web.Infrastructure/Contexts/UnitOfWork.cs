// <copyright file="UnitOfWork.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts;
using Web.Application.Exceptions.Database;

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
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DatabaseUpdateConcurrencyException("A concurrency conflict occurred while saving changes.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new DatabaseUpdateException("A database update exception occurred while saving changes.", ex);
        }
    }
}
