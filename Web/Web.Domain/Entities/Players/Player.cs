// <copyright file="Player.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Players;

/// <summary>
/// Represents a player.
/// </summary>
/// <seealso cref="AuditableEntityBase" />
public sealed class Player : AuditableEntityBase
{
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Player"/> is deleted.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this <see cref="Player"/> is deleted; otherwise, <c>false</c>.
    /// </value>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the name of the <see cref="Player"/>.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> that represents the name of the <see cref="Player"/>.
    /// </value>
    public required string Name { get; init; }

    /// <summary>
    /// Gets a <see cref="string"/> that represents the user account identifier that is associated with this <see cref="Player"/>.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the user account identifier that is associated with this <see cref="Player"/>.
    /// </value>
    public required string UserAccountId { get; init; }

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
