// <copyright file="IUserAccountTokenService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Services.Users;

/// <summary>
/// Represents a JSON Web Token (JWT) that contains access and refresh tokens for an authenticated user.
/// </summary>
/// <param name="AccessToken">
/// Specifies a <see cref="string"/> representing the access token used to authenticate API requests.
/// </param>
/// <param name="RefreshToken">
/// Specifies a <see cref="string"/> representing the refresh token used to obtain new access tokens after expiration.
/// </param>
/// <param name="SessionId">
/// Specifies a <see cref="Guid"/> representing the unique identifier of the user session associated with the token.
/// </param>
public sealed record JwtToken(
    string AccessToken,
    string RefreshToken,
    Guid SessionId);

/// <summary>
/// Represents the parameters used to generate a JSON Web Token (JWT) for a user account.
/// </summary>
/// <param name="UserAccountId">
/// Specifies a <see cref="string"/> representing the unique identifier of the user account for which the JWT is being generated.
/// </param>
/// <param name="Username">
/// Specifies a <see cref="string"/> representing the username associated with the user account.
/// </param>
public sealed record JwtParameters(
    string UserAccountId,
    string Username);

/// <summary>
/// Defines an interface that provides operations for generating and managing JSON Web Tokens (JWTs) for user authentication.
/// </summary>
public interface IUserAccountTokenService
{
    /// <summary>
    /// Generates a new <see cref="JwtToken"/> using the specified <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">
    /// Specifies a <see cref="JwtParameters"/> instance containing the user information required to generate the token.
    /// </param>
    /// <returns>
    /// Returns a <see cref="JwtToken"/> containing the generated access and refresh tokens.
    /// </returns>
    JwtToken GenerateJwt(JwtParameters parameters);

    /// <summary>
    /// Hashes the specified <paramref name="refreshToken"/> for secure storage and comparison.
    /// </summary>
    /// <param name="refreshToken">
    /// Specifies a <see cref="string"/> representing the refresh token to hash.
    /// </param>
    /// <returns>
    /// Returns a <see cref="string"/> representing the hashed refresh token.
    /// </returns>
    string HashRefreshToken(string refreshToken);
}
