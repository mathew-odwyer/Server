namespace Winterhaven.API.Core.Domain.ValueObjects.Users;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents a user token that contains access and refresh tokens for an authenticated user.
/// </summary>
/// <param name="AccessToken">
///   Specifies a <see cref="string"/> representing the access token used to authenticate API requests.
/// </param>
/// <param name="RefreshToken">
///   Specifies a <see cref="string"/> representing the refresh token used to obtain new access tokens after expiration.
/// </param>
/// <param name="AccessTokenExpiryDate">
///   Specifies a <see cref="DateTime"/> representing the date and time the access token will expire.
/// </param>
/// <param name="RefreshTokenExpiryDate">
///   Specifies a <see cref="DateTime"/> representing the date and time the refresh token will expire.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UserToken(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiryDate,
    DateTime RefreshTokenExpiryDate);