// <copyright file="UserAccount.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;
using Web.Domain.Entities.Players;

/// <summary>
/// Represents a user account with various properties related to authentication and user details.
/// </summary>
/// <seealso cref="IdentityUser" />
public sealed class UserAccount : IdentityUser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    public UserAccount()
    {
        this.Players = [];
    }

    /// <summary>
    /// Gets the players associated with this <see cref="UserAccount"/>.
    /// </summary>
    /// <value>
    /// The players associated with this <see cref="UserAccount"/>.
    /// </value>
    public ICollection<Player> Players { get; }
}
