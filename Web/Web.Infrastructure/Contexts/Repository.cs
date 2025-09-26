// <copyright file="Repository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts;
using Web.Domain.Entities;

/// <summary>
/// Provides the implementation of repository operations for entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity.
/// </typeparam>
[ExcludeFromCodeCoverage]
internal abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : EntityBase
{
    /// <summary>
    /// The database context.
    /// </summary>
    private readonly DbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">
    /// Specifies a <see cref="DbContext"/> instance to use for repository operations.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> is <c>null</c>.
    /// </exception>
    public Repository(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await this.context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Delete(TEntity entity)
    {
        this.context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc/>
    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        this.context.Set<TEntity>().RemoveRange(entities);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await this.context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public IEnumerable<TEntity> GetAll()
    {
        return this.context.Set<TEntity>();
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> Query()
    {
        return this.context.Set<TEntity>().AsQueryable();
    }
}
