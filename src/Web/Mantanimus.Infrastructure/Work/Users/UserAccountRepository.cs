// <copyright file="UserAccountRepository.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure.Work.Users;

using System;
using Mantanimus.Core.Application.Work.Users;
using Mantanimus.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

internal sealed class UserAccountRepository : Repository<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(DbContext context)
        : base(context)
    {
    }

    public UserAccount? GetById(Guid id)
    {
        return this.Query().FirstOrDefault(x => x.Id == id);
    }
}
