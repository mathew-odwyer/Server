namespace Winterhaven.API.Core.Domain.Entities.Users;

using System;

/// <summary>
///   Represents a user session (for single-session management).
/// </summary>
/// <seealso cref="EntityBase"/>
public class UserSessionToken : AuditableEntityBase
{
    /// <summary>
    ///   Gets or sets a <see cref="DateTime"/> that represents when the session will expire.
    /// </summary>
    /// <value>
    ///   A <see cref="DateTime"/> that represents when the session will expire.
    /// </value>
    public DateTime AccessTokenExpirationDate { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="string"/> that represents the hashed refresh token.
    /// </summary>
    /// <value>
    ///   A <see cref="string"/> that represents the hashed refresh token.
    /// </value>
    public required string HashedRefreshToken { get; init; }

    /// <summary>
    ///   Gets a value indicating whether this session is revoked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this session is revoked; otherwise, <c>false</c>.
    /// </value>
    public bool IsRevoked { get; set; }

    /// <summary>
    ///   Gets or sets a <see cref="DateTime"/> that represents when the refresh token used to refresh the session will expire.
    /// </summary>
    /// <value>
    ///   A <see cref="DateTime"/> that represents when the refresh token used to refresh the session will expire.
    /// </value>
    public DateTime RefreshTokenExpirationDate { get; set; }

    /// <summary>
    ///   Gets the user account associated with this session.
    /// </summary>
    /// <value>
    ///   The user account associated with this session.
    /// </value>
    public virtual required UserAccount UserAccount { get; init; }
}