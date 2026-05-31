using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Domain.ValueObjects.Users;
using Winterhaven.API.Infrastructure.Options.Security;

namespace Winterhaven.API.Infrastructure.Services.Security;

internal sealed class SecureTokenFactory : ISecureTokenFactory
{
    private readonly ILogger<SecureTokenFactory> logger;

    private readonly IOptions<JwtOptions> options;

    public SecureTokenFactory(
        ILogger<SecureTokenFactory> logger,
        IOptions<JwtOptions> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public string GenerateSecureToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    public UserToken GenerateUserToken(UserTokenParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        string accessToken = GenerateAccessToken(parameters);
        string refreshToken = GenerateSecureToken();

        return new UserToken(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiryDate: DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpiryMinutes),
            RefreshTokenExpiryDate: DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpiryDays));
    }

    private string GenerateAccessToken(UserTokenParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        logger.LogDebug("Generating JWT for user: {Username}", parameters.Username);

        var claims = new[]
        {
            new Claim("identifier", parameters.UserAccountId.ToString()),
            new Claim("username", parameters.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        logger.LogDebug("Successfully generated JWT token for user: {Username}", parameters.Username);

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }
}