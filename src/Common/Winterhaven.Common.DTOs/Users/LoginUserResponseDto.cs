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
/// <param name="ExpirationSeconds">
///   The expiration time (in seconds) of the access token.
/// </param>
public sealed record LoginUserResponseDto(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);