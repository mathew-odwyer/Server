namespace Winterhaven.Gateway.Core.Application.Services.Sessions;

public interface ISessionAuthenticator
{
    bool IsAuthenticated { get; }
    
    void Authenticate(string accessToken);
 
    void Invalidate();
}