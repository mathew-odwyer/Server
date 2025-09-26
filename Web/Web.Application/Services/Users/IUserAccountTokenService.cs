// <copyright file="IUserAccountTokenService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Services.Users;

using Web.Domain.Entities.Users;

public sealed record JwtToken(
    string AccessToken,
    string RefreshToken,
    Guid SessionId);

public interface IUserAccountTokenService
{
    JwtToken GenerateJwt(UserAccount userAccount);

    string HashRefreshToken(string refreshToken);
}
