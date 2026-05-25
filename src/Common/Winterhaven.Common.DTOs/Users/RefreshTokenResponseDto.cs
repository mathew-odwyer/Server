namespace Winterhaven.Common.DTOs.Users;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record RefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);