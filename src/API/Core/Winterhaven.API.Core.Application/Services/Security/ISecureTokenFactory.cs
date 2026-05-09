namespace Winterhaven.API.Core.Application.Services.Security;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a user token that contains access and refresh tokens for an authenticated user.
/// </summary>
/// <param name="AccessToken">
/// Specifies a <see cref="string"/> representing the access token used to authenticate API requests.
/// </param>
/// <param name="RefreshToken">
/// Specifies a <see cref="string"/> representing the refresh token used to obtain new access tokens
/// after expiration.
/// </param>
/// <param name="AccessTokenExpiryDate">
/// Specifies a <see cref="DateTime"/> representing the date and time the access token will expire.
/// </param>
/// <param name="RefreshTokenExpiryDate">
/// Specifies a <see cref="DateTime"/> representing the date and time the refresh token will expire.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UserToken(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiryDate,
    DateTime RefreshTokenExpiryDate);

/// <summary>
/// Represents the parameters used to generate a user token for a user account.
/// </summary>
/// <param name="UserAccountId">
/// Specifies a <see cref="string"/> representing the unique identifier of the user account for
/// which the JWT is being generated.
/// </param>
/// <param name="Username">
/// Specifies a <see cref="string"/> representing the username associated with the user account.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UserTokenParameters(
    Guid UserAccountId,
    string Username);

/// <summary>
/// Defines an interface that provides functionality to generate secure tokens (such as JSON Web Tokens).
/// </summary>
public interface ISecureTokenFactory
{
    /// <summary>
    /// Generates a new <see cref="UserToken"/> using the specified <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">
    /// Specifies a <see cref="UserTokenParameters"/> instance containing the user information
    /// required to generate the token.
    /// </param>
    /// <returns>
    /// Returns a <see cref="UserToken"/> containing the generated access and refresh tokens.
    /// </returns>
    UserToken GenerateUserToken(UserTokenParameters parameters);

    /// <summary>
    /// Generates a new secure token.
    /// </summary>
    /// <returns>Returns a <see cref="string"/> representing the generated secure token.</returns>
    string GenerateSecureToken();
}