using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Domain.Entities;

namespace Winterhaven.API.Infrastructure.Work;

internal abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    where TEntity : EntityBase
{
    private readonly DbContext context;

    protected RepositoryBase(DbContext context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

    public void Delete(TEntity entity)
        => context.Set<TEntity>().Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities)
        => context.Set<TEntity>().RemoveRange(entities);

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);

    public IEnumerable<TEntity> GetAll()
        => context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

    public IQueryable<TEntity> Query()
        => context.Set<TEntity>().AsQueryable();
}