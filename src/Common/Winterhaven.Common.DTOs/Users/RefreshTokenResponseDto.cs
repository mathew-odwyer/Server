namespace Winterhaven.Common.DTOs.Users;

public sealed record RefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);