// <copyright file="IRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Work;

using System.Linq.Expressions;
using Winterhaven.Core.Domain.Entities;

/// <summary>
/// Represents an interface that defines a repository for a specific entity type.
/// </summary>
/// <typeparam name="TEntity">
/// Specifies the type of the entity for the repository.
/// </typeparam>
public interface IRepository<TEntity>
    where TEntity : EntityBase
{
    /// <summary>
    /// Adds an entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">
    /// Specifies a <typeparamref name="TEntity"/> that represents the entity to add.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns a <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a range of entities to the repository asynchronously.
    /// </summary>
    /// <param name="entities">
    /// Specifies an <see cref="IEnumerable{TEntity}"/> that represents the entities to add.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns a <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">
    /// Specifies a <typeparamref name="TEntity"/> that represents the entity to delete.
    /// </param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes a range of entities from the repository.
    /// </summary>
    /// <param name="entities">
    /// Specifies an <see cref="IEnumerable{TEntity}"/> that represents the entities to delete.
    /// </param>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Finds entities in the repository that match the specified predicate.
    /// </summary>
    /// <param name="predicate">
    /// Specifies an <see cref="Expression{TDelegate}"/> that represents the predicate to filter entities.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns a <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains the collection of entities that match the specified predicate.
    /// </returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities from the repository.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="IEnumerable{TEntity}"/> that represents all entities in the repository.
    /// </returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Gets an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">
    /// Specifies a <see cref="Guid"/> that represents the identifier of the entity to retrieve.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns a <see cref="Task{TEntity}"/> that represents the asynchronous operation. The task result contains the entity with the specified identifier, or null if no such entity exists.
    /// </returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the repository.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="IQueryable{T}"/> that represents the queryable instance.
    /// </returns>
    IQueryable<TEntity> Query();
}
