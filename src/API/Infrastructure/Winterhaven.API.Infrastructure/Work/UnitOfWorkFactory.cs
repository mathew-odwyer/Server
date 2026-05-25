namespace Winterhaven.API.Infrastructure.Work;

using Microsoft.EntityFrameworkCore;
using System;
using Winterhaven.API.Core.Application.Work;

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