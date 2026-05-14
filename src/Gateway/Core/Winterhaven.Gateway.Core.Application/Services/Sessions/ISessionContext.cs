namespace Winterhaven.Gateway.Core.Application.Services.Sessions;

public interface ISessionContext
{
    string? AccessToken { get; }

    bool IsAuthenticated { get; }
}
