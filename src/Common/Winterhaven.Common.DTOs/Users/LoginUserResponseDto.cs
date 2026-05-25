namespace Winterhaven.Common.DTOs.Users;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record LoginUserResponseDto(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);