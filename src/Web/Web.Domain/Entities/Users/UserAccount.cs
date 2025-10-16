// <copyright file="UserAccount.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;
using Web.Domain.Entities.Players;

/// <summary>
/// Represents a user account.
/// </summary>
/// <seealso cref="IdentityUser" />
public class UserAccount : IdentityUser<Guid>
{
    /// <summary>
    /// Gets the player linked to this <see cref="UserAccount"/>.
    /// </summary>
    /// <value>
    /// The player linked to this <see cref="UserAccount"/>.
    /// </value>
    public virtual required Player Player { get; set; }
}
