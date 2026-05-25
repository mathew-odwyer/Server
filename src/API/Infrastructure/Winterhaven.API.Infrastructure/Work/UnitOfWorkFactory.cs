namespace Winterhaven.API.Infrastructure.Work;

using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Application.Work;

[ExcludeFromCodeCoverage]
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