using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Users;

/// <summary>
///   Represents a user login response data transfer object.
/// </summary>
/// <param name="AccessToken">
///   The access token for the user that has logged in.
/// </param>
/// <param name="RefreshToken">
///   The refresh token for the user who has logged in.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserResponseDto(
    string AccessToken,
    string RefreshToken);
