// <copyright file="UnitOfWork.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts;
using Web.Application.Exceptions;

/// <summary>
/// Provides the implementation of the unit of work pattern to manage repository instances and coordinate changes to the database.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// The database context.
    /// </summary>
    private readonly DbContext? context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">
    /// Specifies a <see cref="DbContext"/> instance to use for data operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> is <c>null</c>.
    /// </exception>
    public UnitOfWork(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
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
