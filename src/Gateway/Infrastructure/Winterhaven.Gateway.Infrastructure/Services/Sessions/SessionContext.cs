namespace Winterhaven.Gateway.Infrastructure.Services.Sessions;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Linq;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Core.Application.Services.Users;

internal sealed class SessionContext : ISessionContext, ISessionAuthenticator, IUserAccountContext
{
    private readonly ILogger<SessionContext> logger;

    public SessionContext(ILogger<SessionContext> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string? AccessToken { get; private set; }

    public string? Username
    {
        get { return this.ParseClaim("username", value => value); }
    }

    public bool IsAuthenticated
    {
        get { return !string.IsNullOrWhiteSpace(this.AccessToken); }
    }

    public void Authenticate(string accessToken)
    {
        this.AccessToken = accessToken;
        this.logger.LogDebug("User session authenticated: '{Username}'", this.Username);
    }

    public void Invalidate()
    {
        this.logger.LogDebug("Invalidating user session for username: '{Username}'", this.Username);
        this.AccessToken = null;
    }

    private T? ParseClaim<T>(string type, Func<string, T> selector)
    {
        if (string.IsNullOrWhiteSpace(this.AccessToken))
        {
            return default;
        }

        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(this.AccessToken);

        var claim = jwt.Claims.FirstOrDefault(c => c.Type == type);
        return claim is not null ? selector(claim.Value) : default;
    }
}
