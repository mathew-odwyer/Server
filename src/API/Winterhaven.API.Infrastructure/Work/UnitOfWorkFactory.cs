using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work;

namespace Winterhaven.API.Infrastructure.Work;

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
