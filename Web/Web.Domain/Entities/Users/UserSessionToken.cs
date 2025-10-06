// <copyright file="UserSessionToken.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Users;

/// <summary>
/// Represents a user session (for single-session management).
/// </summary>
/// <seealso cref="AuditableEntityBase" />
public sealed class UserSessionToken : AuditableEntityBase
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
    /// Gets a <see cref="Guid"/> that represents the user session identifier.
    /// </summary>
    /// <value>
    /// A <see cref="Guid"/> that represents the user session identifier.
    /// </value>
    /// <remarks>
    /// Each user session is unique, as a result the session identifier is used to determine whether this session is current and active in middleware.
    /// </remarks>
    public required Guid SessionId { get; init; }

    /// <summary>
    /// Gets a <see cref="string"/> that represents the user account identifier associated with this session.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the user account identifier associated with this session.
    /// </value>
    public required string UserAccountId { get; init; }
}
