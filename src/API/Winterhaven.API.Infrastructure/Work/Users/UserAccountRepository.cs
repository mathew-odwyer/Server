using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Infrastructure.Work.Users;

[ExcludeFromCodeCoverage]
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
