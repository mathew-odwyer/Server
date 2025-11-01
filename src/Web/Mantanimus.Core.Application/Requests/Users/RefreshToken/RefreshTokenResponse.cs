// <copyright file="RefreshTokenResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Requests.Users.RefreshToken;

using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Represents a response that contain the JSON Web Token for the current <see cref="UserAccount"/>.
/// </summary>
/// <param name="AccessToken">
/// The new access token for the <see cref="UserAccount"/>.
/// </param>
/// <param name="RefreshToken">
/// The new refresh token for the <see cref="UserAccount"/>.
/// </param>
/// <param name="ExpirationSeconds">
/// The access token expiry (in seconds).
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    double ExpirationSeconds);
