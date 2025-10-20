// <copyright file="ISecureTokenFactory.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Services.Security;

/// <summary>
/// Represents a JSON Web Token (JWT) that contains access and refresh tokens for an authenticated user.
/// </summary>
/// <param name="AccessToken">
/// Specifies a <see cref="string"/> representing the access token used to authenticate API requests.
/// </param>
/// <param name="RefreshToken">
/// Specifies a <see cref="string"/> representing the refresh token used to obtain new access tokens after expiration.
/// </param>
public sealed record JwtToken(
    string AccessToken,
    string RefreshToken);

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
    Guid UserAccountId,
    string Username);

/// <summary>
/// Defines an interface that provides functionality to generate secure tokens (such as JSON Web Tokens).
/// </summary>
public interface ISecureTokenFactory
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
    /// Genereates a new secure token.
    /// </summary>
    /// <returns>
    /// Returns a <see cref="string"/> representing the generated secure token.
    /// </returns>
    string GenerateSecureToken();
}
