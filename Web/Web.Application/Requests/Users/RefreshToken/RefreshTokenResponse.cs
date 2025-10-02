// <copyright file="RefreshTokenResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RefreshToken;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken);
