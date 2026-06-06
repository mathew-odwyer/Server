using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

[ExcludeFromCodeCoverage]
internal sealed class UserTokenParser : IUserTokenParser
{
    public UserSession ParseUserToken(string userToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userToken);

        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(userToken);
        var claims = jwt.Claims;

        return new UserSession(
            UserAccountId: Guid.Parse(claims.First(c => c.Type == "identifier").Value),
            AccessToken: userToken,
            ExpiresAt: jwt.ValidTo);
    }
}
