// <copyright file="IUnitOfWork.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts;

using System.Threading.Tasks;

/// <summary>
/// Represents an interface that defines a unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work asynchronously.
    /// </summary>
    /// <returns>
    /// Returns a <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    Task SaveAsync(CancellationToken cancellationToken = default);
}
