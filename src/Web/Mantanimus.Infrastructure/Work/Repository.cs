// <copyright file="Repository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Work;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mantanimus.Core.Application.Work;
using Mantanimus.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

internal abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : EntityBase
{
    private readonly DbContext context;

    public Repository(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await this.context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public void Delete(TEntity entity)
    {
        this.context.Set<TEntity>().Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        this.context.Set<TEntity>().RemoveRange(entities);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await this.context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return this.context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
    }

    public IQueryable<TEntity> Query()
    {
        return this.context.Set<TEntity>().AsQueryable();
    }
}
