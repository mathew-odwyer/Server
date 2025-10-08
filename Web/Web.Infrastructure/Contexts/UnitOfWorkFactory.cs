// <copyright file="UnitOfWorkFactory.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts;

using System;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts;

internal sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbContext context;

    public UnitOfWorkFactory(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(this.context);
    }
}
