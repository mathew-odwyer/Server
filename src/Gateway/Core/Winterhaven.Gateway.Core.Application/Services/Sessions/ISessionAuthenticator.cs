namespace Winterhaven.Gateway.Core.Application.Services.Sessions;

public interface ISessionAuthenticator
{
    void Authenticate(string accessToken);
}
