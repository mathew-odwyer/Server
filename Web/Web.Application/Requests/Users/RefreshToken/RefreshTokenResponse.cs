// <copyright file="RefreshTokenResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RefreshToken;

using System.Diagnostics.CodeAnalysis;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a response that contain the JSON Web Token for the current <see cref="UserAccount"/>.
/// </summary>
/// <param name="AccessToken">
/// The new access token for the <see cref="UserAccount"/>.
/// </param>
/// <param name="RefreshToken">
/// The new refresh token for the <see cref="UserAccount"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken);
