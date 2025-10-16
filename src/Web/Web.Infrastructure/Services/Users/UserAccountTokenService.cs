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

        string accessToken = this.GenerateAccessToken(parameters);
        string refreshToken = this.GenerateSecureToken();

        return new JwtToken(
            AccessToken: accessToken,
            RefreshToken: refreshToken);
    }

    public string GenerateSecureToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        string token = Convert.ToBase64String(bytes);

        return token;
    }

    public string HashSecureToken(string token)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(token);
        byte[] hashedBytes = SHA256.HashData(bytes);
        string hashedCode = Convert.ToBase64String(hashedBytes);

        return hashedCode;
    }

    private string GenerateAccessToken(JwtParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        this.logger.LogInformation("Generating JWT for user: {Username}", parameters.Username);

        var claims = new[]
        {
            new Claim("identifier", parameters.UserAccountId.ToString()),
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
}
