namespace Winterhaven.API.Presentation.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Winterhaven.API.Presentation.Options.Security;

[ExcludeFromCodeCoverage]
internal sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IOptions<ApiOptions> apiOptions;

    public ApiKeyAuthenticationHandler(
        IOptions<ApiOptions> apiOptions,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // If there's no API Key, try authenticating with the next scheme (JWT).
        if (!this.Request.Headers.TryGetValue("X-API-KEY", out var key) || string.IsNullOrWhiteSpace(key) || key != this.apiOptions.Value.Key)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim("api_key", "true"),
        };

        var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

        // From here, if the server attempts to make a call to a MediatR request handler on behalf of a user that is [Authorize] it will fail However, this will give the server authority to make requests for things like fetching maps, NPC data, etc.
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}