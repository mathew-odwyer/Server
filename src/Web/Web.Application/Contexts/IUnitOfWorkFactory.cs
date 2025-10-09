// <copyright file="IUnitOfWorkFactory.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts;

/// <summary>
/// Represents an interface that defines a factory for creating units of work.
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a new unit of work.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="IUnitOfWork"/> that represents the newly created unit of work.
    /// </returns>
    IUnitOfWork CreateUnitOfWork();
}
