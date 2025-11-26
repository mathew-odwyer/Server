// <copyright file="LoginUserResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.LoginUser;

using System.Diagnostics.CodeAnalysis;
using Winterhaven.Core.Domain.Entities.Users;

/// <summary>
/// Represents a response that contain tokens used to authorized a <see cref="UserAccount"/>.
/// </summary>
/// <param name="AccessToken">
/// The access token used to authorize the user.
/// </param>
/// <param name="RefreshToken">
/// The refresh token used to obtain a new access token.
/// </param>
/// <param name="ExpirationSeconds">
/// The access token expiry (in seconds).
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);
