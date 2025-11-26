// <copyright file="UnitOfWorkFactory.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Work;

using System;
using Winterhaven.Core.Application.Work;
using Microsoft.EntityFrameworkCore;

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
