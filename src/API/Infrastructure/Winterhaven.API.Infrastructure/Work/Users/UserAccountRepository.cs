using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Infrastructure.Work.Users;

internal sealed class UserAccountRepository : RepositoryBase<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(DbContext context)
        : base(context)
    {
    }

    public UserAccount? GetById(Guid id) => Query().FirstOrDefault(x => x.Id == id);
}