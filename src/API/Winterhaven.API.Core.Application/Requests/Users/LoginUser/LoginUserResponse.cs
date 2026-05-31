using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Core.Application.Requests.Users.LoginUser;

/// <summary>
///   Represents a response that contain tokens used to authorized a user account.
/// </summary>
/// <param name="AccessToken">
///   The access token used to authorize the user.
/// </param>
/// <param name="RefreshToken">
///   The refresh token used to obtain a new access token.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken);
