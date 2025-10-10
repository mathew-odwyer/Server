// <copyright file="LoginUserResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a response that contain tokens used to authorized a <see cref="UserAccount"/>.
/// </summary>
/// <param name="AccessToken">
/// The access token used to handle authorization.
/// </param>
/// <param name="RefreshToken">
/// The refresh token used to handle refreshing the access and refresh tokens.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken,
    [property: JsonPropertyName("refresh_token")]
    string RefreshToken);
