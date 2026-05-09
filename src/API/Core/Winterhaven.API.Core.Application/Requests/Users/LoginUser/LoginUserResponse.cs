namespace Winterhaven.API.Core.Application.Requests.Users.LoginUser;

using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
/// Represents a response that contain tokens used to authorized a <see cref="UserAccount"/>.
/// </summary>
/// <param name="AccessToken">The access token used to authorize the user.</param>
/// <param name="RefreshToken">The refresh token used to obtain a new access token.</param>
/// <param name="ExpirationSeconds">The access token expiry (in seconds).</param>
public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);