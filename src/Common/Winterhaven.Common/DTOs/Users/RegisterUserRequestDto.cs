using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Users;

/// <summary>
///   Represents the data transfer object used to register a new user account.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record RegisterUserRequestDto
{
    /// <summary>
    ///   Gets the email address associated with the new user account.
    /// </summary>
    /// <value>
    ///   The email address associated with the new user account.
    /// </value>
    /// <remarks>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Cannot be <c>null</c> or empty.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must be a valid email address format (e.g., <c>name@example.com</c>).</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public required string EmailAddress { get; init; }

    /// <summary>
    ///   Gets the desired username for the new user account.
    /// </summary>
    /// <value>
    ///   The desired username for the new user account.
    /// </value>
    /// <remarks>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Cannot be <c>null</c>, empty, or consist only of whitespace characters.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must be at least 3 characters long.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must be no more than 12 characters long.</description>
    ///     </item>
    ///     <item>
    ///       <description>Can only contain alphanumeric characters, hyphens ( <c>-</c>), or underscores ( <c>_</c>).</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public required string Username { get; init; }

    /// <summary>
    ///   Gets the password used to secure the new user account.
    /// </summary>
    /// <value>
    ///   The password used to secure the new user account.
    /// </value>
    /// <remarks>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Cannot be <c>null</c> or empty.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must be at least 12 characters long.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must contain at least one uppercase letter.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must contain at least one lowercase letter.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must contain at least one numeric character.</description>
    ///     </item>
    ///     <item>
    ///       <description>Must contain at least one special (non-alphanumeric) character.</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public required string Password { get; init; }
}
