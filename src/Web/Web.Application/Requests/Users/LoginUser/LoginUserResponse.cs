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
/// <param name="ClientToken">
/// The client token used to validate the user.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserResponse(
    [property: JsonPropertyName("client_token")]
    string ClientToken);
