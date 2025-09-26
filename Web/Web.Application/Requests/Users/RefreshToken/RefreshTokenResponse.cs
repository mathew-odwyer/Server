// <copyright file="RefreshTokenResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RefreshToken;

using Web.Application.Services.Users;

public sealed record RefreshTokenResponse(JwtToken Token);
