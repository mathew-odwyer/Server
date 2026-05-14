namespace Winterhaven.Common.DTOs.Users;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the data transfer object for a refresh token request.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record RefreshTokenRequestDto
{
    /// <summary>
    /// Gets the refresh token used to generate a new JWT.
    /// </summary>
    /// <value>
    /// The refresh token used to generate a new JWT.
    /// </value>
    public required string RefreshToken { get; init; }
}