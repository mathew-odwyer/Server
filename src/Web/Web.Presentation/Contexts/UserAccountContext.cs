// <copyright file="UserContext.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Contexts;

using System;
using System.Security.Claims;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Domain.Entities.Users;

internal sealed class UserAccountContext : IUserAccountContext
{
    private readonly IHttpContextAccessor? accessor;

    private readonly IUserAccountRepository userAccountRepository;

    public UserAccountContext(IUserAccountRepository userAccountRepository, IHttpContextAccessor? accessor = null)
    {
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
        this.accessor = accessor;
    }

    public UserAccount? User
    {
        get
        {
            var user = this.accessor?.HttpContext?.User;

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                string? identifier = user.FindFirstValue("identifier");

                if (!string.IsNullOrWhiteSpace(identifier) && Guid.TryParse(identifier, out var userAccountId))
                {
                    return this.userAccountRepository.GetById(userAccountId, default);
                }
            }

            return null;
        }
    }
}
