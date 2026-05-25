namespace Winterhaven.Common.DTOs.Users;

public sealed record LoginUserResponseDto(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);