// <copyright file="UserAccountTokenService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Services.Users;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web.Application.Options.Security;
using Web.Application.Services.Users;

internal sealed class UserAccountTokenService : IUserAccountTokenService
{
    private readonly ILogger<UserAccountTokenService> logger;

    private readonly IOptions<JwtOptions> options;

    public UserAccountTokenService(
        ILogger<UserAccountTokenService> logger,
        IOptions<JwtOptions> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public JwtToken GenerateJwt(JwtParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sessionId = GenerateSessionId();
        string accessToken = this.GenerateAccessToken(parameters, sessionId);
        string refreshToken = this.GenerateRefreshToken();

        return new JwtToken(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            SessionId: sessionId);
    }

    public string HashRefreshToken(string refreshToken)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(refreshToken);
        byte[] hashedBytes = SHA256.HashData(bytes);
        string hashedCode = Convert.ToBase64String(hashedBytes);

        return hashedCode;
    }

    private static Guid GenerateSessionId()
    {
        return Guid.NewGuid();
    }

    private string GenerateAccessToken(JwtParameters parameters, Guid sessionId)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        this.logger.LogInformation("Generating JWT for user: {Username}", parameters.Username);

        var claims = new[]
        {
            new Claim("identifier", parameters.UserAccountId),
            new Claim("session", sessionId.ToString()),
            new Claim("username", parameters.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.options.Value.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this.options.Value.Issuer,
            audience: this.options.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        this.logger.LogInformation("Successfully generated JWT token for user: {Username}", parameters.Username);

        var handler = new JwtSecurityTokenHandler();
        string secureToken = handler.WriteToken(token);

        return secureToken;
    }

    private string GenerateRefreshToken()
    {
        this.logger.LogInformation("Generating refresh token...");

        byte[] refreshTokenBytes = RandomNumberGenerator.GetBytes(32);
        string refreshToken = Convert.ToBase64String(refreshTokenBytes);

        this.logger.LogInformation("Refresh token generated!");

        return refreshToken;
    }
}
