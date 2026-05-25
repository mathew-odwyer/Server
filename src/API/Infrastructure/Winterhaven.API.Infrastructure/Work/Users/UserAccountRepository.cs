namespace Winterhaven.API.Infrastructure.Work.Users;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

internal sealed class UserAccountRepository : RepositoryBase<UserAccount>, IUserAccountRepository
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