// <copyright file="ValidateUserResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.ValidateUser;

using System.Text.Json.Serialization;

public sealed record ValidateUserResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken,
    [property: JsonPropertyName("refresh_token")]
    string RefreshToken);
