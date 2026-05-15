namespace Winterhaven.API.Infrastructure.Services.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Infrastructure.Options.Security;

[ExcludeFromCodeCoverage]
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

    public UserToken GenerateUserToken(UserTokenParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        string accessToken = this.GenerateAccessToken(parameters);
        string refreshToken = this.GenerateSecureToken();

        return new UserToken(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiryDate: DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            RefreshTokenExpiryDate: DateTime.UtcNow.AddDays(this.options.Value.RefreshTokenExpiryDays));
    }

    public string GenerateSecureToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        string token = Convert.ToBase64String(bytes);

        return token;
    }

    private string GenerateAccessToken(UserTokenParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        this.logger.LogDebug("Generating JWT for user: {Username}", parameters.Username);

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

        this.logger.LogDebug("Successfully generated JWT token for user: {Username}", parameters.Username);

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }
}