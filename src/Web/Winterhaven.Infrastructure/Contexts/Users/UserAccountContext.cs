// <copyright file="UserAccountContext.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Contexts.Users;

using System.Security.Claims;
using Winterhaven.Core.Application.Contexts.Users;
using Winterhaven.Core.Application.Work.Users;
using Winterhaven.Core.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

internal sealed class UserAccountContext : IUserAccountContext
{
    private readonly IHttpContextAccessor? accessor;

    private readonly IUserAccountRepository userAccountRepository;

    public UserAccountContext(IUserAccountRepository userAccountRepository, IHttpContextAccessor? accessor = null)
    {
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
        this.accessor = accessor;
    }

    public UserAccount User
    {
        get
        {
            var user = this.accessor?.HttpContext?.User;

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                string? identifier = user.FindFirstValue("identifier");

                if (!string.IsNullOrWhiteSpace(identifier) && Guid.TryParse(identifier, out var userAccountId))
                {
                    return this.userAccountRepository.GetById(userAccountId)!;
                }
            }

            return null!;
        }
    }
}
