// <copyright file="UnitOfWorkFactory.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts;

/// <summary>
/// Provides the implementation of a factory to create instances of <see cref="IUnitOfWork"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    /// <summary>
    /// The database context.
    /// </summary>
    private readonly DbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkFactory"/> class.
    /// </summary>
    /// <param name="context">
    /// Specifies a <see cref="DbContext"/> instance to be used for creating <see cref="IUnitOfWork"/> instances.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> is <c>null</c>.
    /// </exception>
    public UnitOfWorkFactory(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public IUnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(this.context);
    }
}
