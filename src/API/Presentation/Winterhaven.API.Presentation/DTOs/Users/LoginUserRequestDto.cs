namespace Winterhaven.API.Presentation.DTOs.Users;

using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
/// Represents the data transfer object used to authenticate an existing <see cref="UserAccount"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record LoginUserRequestDto
{
    /// <summary>
    /// The username associated with the <see cref="UserAccount"/>.
    /// </summary>
    /// <value>
    /// The username associated with the <see cref="UserAccount"/>.
    /// </value>
    /// <remarks>
    /// <para>Validation rules:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>Cannot be <c>null</c>, empty, or consist only of whitespace characters.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public required string Username { get; init; }

    /// <summary>
    /// The password used to authenticate the <see cref="UserAccount"/>.
    /// </summary>
    /// <value>
    /// The password used to authenticate the <see cref="UserAccount"/>.
    /// </value>
    /// <remarks>
    /// <para>Validation rules:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>Cannot be <c>null</c>, empty, or consist only of whitespace characters.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public required string Password { get; init; }
}
