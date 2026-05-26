namespace Winterhaven.Common.DTOs.Users;

/// <summary>
///   Represents the data transfer object used to authenticate an existing user account.
/// </summary>
public sealed record LoginUserRequestDto
{
    /// <summary>
    ///   The username associated with the user account.
    /// </summary>
    /// <value>
    ///   The username associated with the user account.
    /// </value>
    /// <remarks>
    ///   <para>Validation rules:</para>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Cannot be <c>null</c>, empty, or consist only of whitespace characters.</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public required string Username { get; init; }

    /// <summary>
    ///   The password used to authenticate the user account.
    /// </summary>
    /// <value>
    ///   The password used to authenticate the user account.
    /// </value>
    /// <remarks>
    ///   <para>Validation rules:</para>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Cannot be <c>null</c>, empty, or consist only of whitespace characters.</description>
    ///     </item>
    ///   </list>
    /// </remarks>
    public required string Password { get; init; }
}