// <copyright file="IUserAccountTokenService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Services.Users;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record JwtToken(
    string AccessToken,
    string RefreshToken,
    Guid SessionId);

[ExcludeFromCodeCoverage]
public sealed record JwtParameters(
    string UserAccountId,
    string Username);

public interface IUserAccountTokenService
{
    JwtToken GenerateJwt(JwtParameters parameters);

    string HashRefreshToken(string refreshToken);
}
