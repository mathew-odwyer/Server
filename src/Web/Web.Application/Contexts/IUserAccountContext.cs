// <copyright file="IUserAccountContext.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Contexts;

using Web.Domain.Entities.Users;

public interface IUserAccountContext
{
    UserAccount? User { get; }
}
