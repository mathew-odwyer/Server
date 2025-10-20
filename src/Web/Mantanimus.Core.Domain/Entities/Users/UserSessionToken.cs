// <copyright file="UserSessionToken.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Represents a user session (for single-session management).
/// </summary>
/// <seealso cref="EntityBase" />
public class UserSessionToken : AuditableEntityBase
{
    /// <summary>
    /// Gets or sets a <see cref="DateTime"/> that represents when the the session will expire.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> that represents when the the session will expire.
    /// </value>
    /// <remarks>
    /// The expiration date is linked to the expiration date of the access token.
    /// </remarks>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the hashed refresh token.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the hashed refresh token.
    /// </value>
    public required string HashedRefreshToken { get; init; }

    /// <summary>
    /// Gets a value indicating whether this session is revoked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this session is revoked; otherwise, <c>false</c>.
    /// </value>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets the user account associated with this session.
    /// </summary>
    /// <value>
    /// The user account associated with this session.
    /// </value>
    public virtual required UserAccount UserAccount { get; init; }
}
