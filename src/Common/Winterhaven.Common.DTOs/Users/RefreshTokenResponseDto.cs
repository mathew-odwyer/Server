using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Users;

/// <summary>
///   Represents a data transfer object that contains information pertaining to a user who has refreshed their tokens.
/// </summary>
/// <param name="AccessToken">
///   The new access token for the user.
/// </param>
/// <param name="RefreshToken">
///   The new refresh token for the user.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken);
