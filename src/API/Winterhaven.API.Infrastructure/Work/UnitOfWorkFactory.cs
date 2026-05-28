using System;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work;

namespace Winterhaven.API.Infrastructure.Work;

internal sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbContext context;

    public UnitOfWorkFactory(DbContext context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

    public IUnitOfWork CreateUnitOfWork() => new UnitOfWork(context);
}