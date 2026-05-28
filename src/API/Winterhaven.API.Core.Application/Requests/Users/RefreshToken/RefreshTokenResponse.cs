namespace Winterhaven.API.Core.Application.Requests.Users.RefreshToken;
/// <summary>
///   Represents a response that contain the JSON Web Token for the current user account.
/// </summary>
/// <param name="AccessToken">
///   The new access token for the user account.
/// </param>
/// <param name="RefreshToken">
///   The new refresh token for the user account.
/// </param>
/// <param name="ExpirationSeconds">
///   The access token expiry (in seconds).
/// </param>
public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);