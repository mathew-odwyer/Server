namespace Winterhaven.API.Core.Application.Work;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Domain.Entities;

/// <summary>
///   Represents an interface that defines a repository for a specific entity type.
/// </summary>
/// <typeparam name="TEntity">
///   Specifies the type of the entity for the repository.
/// </typeparam>
public interface IRepository<TEntity>
    where TEntity : EntityBase
{
    /// <summary>
    ///   Adds an entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">
    ///   Specifies a <typeparamref name="TEntity"/> that represents the entity to add.
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    ///   Returns a <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///   Adds a range of entities to the repository asynchronously.
    /// </summary>
    /// <param name="entities">
    ///   Specifies an <see cref="IEnumerable{TEntity}"/> that represents the entities to add.
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    ///   Returns a <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    ///   Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">
    ///   Specifies a <typeparamref name="TEntity"/> that represents the entity to delete.
    /// </param>
    void Delete(TEntity entity);

    /// <summary>
    ///   Deletes a range of entities from the repository.
    /// </summary>
    /// <param name="entities">
    ///   Specifies an <see cref="IEnumerable{TEntity}"/> that represents the entities to delete.
    /// </param>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    ///   Gets all entities from the repository.
    /// </summary>
    /// <returns>
    ///   Returns an <see cref="IEnumerable{TEntity}"/> that represents all entities in the repository.
    /// </returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    ///   Gets an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">
    ///   Specifies a <see cref="Guid"/> that represents the identifier of the entity to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    ///   Returns a <see cref="Task{TEntity}"/> that represents the asynchronous operation. The task result contains the entity with the specified identifier, or null if no such entity exists.
    /// </returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}