// <copyright file="UserAccount.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Represents a user account for an individual user within the system.
/// </summary>
/// <seealso cref="AuditableEntityBase" />
public class UserAccount : AuditableEntityBase
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the email address associated with the user account.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> representing the email address associated with the user account.
    /// </value>
    /// <remarks>
    /// The email address is normalized to uppercase to ensure consistency and uniqueness.
    /// </remarks>
    public required string EmailAddress { get; init; }

    /// <summary>
    /// Gets a <see cref="string"/> representing the username associated with the user account.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> representing the username associated with the user account.
    /// </value>
    /// <remarks>
    /// The username is normalized to uppercase to ensure consistency and uniqueness.
    /// </remarks>
    public required string Username { get; init; }
}
