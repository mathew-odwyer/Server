// <copyright file="UserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Contexts.Users;

using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Web.Application.Contexts.Users;
using Web.Domain.Entities.Users;

internal sealed class UserAccountRepository : IUserAccountRepository
{
    private readonly DbContext context;

    public UserAccountRepository(DbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public UserAccount? GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return this.context.Set<UserAccount>().FirstOrDefault(x => x.Id == id);
    }
}
