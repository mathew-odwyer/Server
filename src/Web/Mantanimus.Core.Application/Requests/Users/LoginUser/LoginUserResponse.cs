// <copyright file="LoginUserResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Requests.Users.LoginUser;

using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Represents a response that contain tokens used to authorized a <see cref="UserAccount"/>.
/// </summary>
/// <param name="AccessToken">
/// The access token used to authorize the user.
/// </param>
/// <param name="RefreshToken">
/// The refresh token used to obtain a new access token.
/// </param>
/// <param>
/// The expiration date and time of the access token.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpirationDate);
