// <copyright file="IUnitOfWork.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Work;

/// <summary>
/// Defines an interface that represents a unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Represents a <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Returns a <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    Task SaveAsync(CancellationToken cancellationToken = default);
}
