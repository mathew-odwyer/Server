// <copyright file="Player.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Players;

using Web.Domain.Entities.Users;

/// <summary>
/// Represents a player.
/// </summary>
/// <seealso cref="AuditableEntityBase" />
public sealed class Player : AuditableEntityBase
{
    private string? name;

    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the name of the <see cref="Player"/>.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> that represents the name of the <see cref="Player"/>.
    /// </value>
    public required string Name
    {
        get
        {
            return this.name!;
        }

        set
        {
            this.name = value;
            this.NormalizedName = this.name.ToUpperInvariant();
        }
    }

    public string NormalizedName { get; private set; } = null!;

    /// <summary>
    /// Gets the <see cref="Users.UserAccount"/> that owns this <see cref="Player"/>.
    /// </summary>
    /// <value>
    /// The <see cref="Users.UserAccount"/> that owns this <see cref="Player"/>.
    /// </value>
    public required UserAccount UserAccount { get; init; }

    /// <summary>
    /// Gets or sets an <see cref="int"/> that represents the X-coordinate of the <see cref="Player"/>.
    /// </summary>
    /// <value>
    /// The <see cref="int"/> that represents the X-coordinate of the <see cref="Player"/>.
    /// </value>
    public int X { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="int"/> that represents the Y-coordinate of the <see cref="Player"/>.
    /// </summary>
    /// <value>
    /// The <see cref="int"/> that represents the Y-coordinate of the <see cref="Player"/>.
    /// </value>
    public int Y { get; set; }
}
