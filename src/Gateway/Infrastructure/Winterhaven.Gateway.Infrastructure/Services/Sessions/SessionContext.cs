namespace Winterhaven.Gateway.Infrastructure.Services.Sessions;

using Winterhaven.Gateway.Core.Application.Services.Sessions;

internal sealed class SessionContext : ISessionContext, ISessionAuthenticator
{
    public string? AccessToken { get; private set; }

    public bool IsAuthenticated
    {
        get { return !string.IsNullOrEmpty(this.AccessToken); }
    }

    public void Authenticate(string accessToken)
    {
        this.AccessToken = accessToken;
    }
}
