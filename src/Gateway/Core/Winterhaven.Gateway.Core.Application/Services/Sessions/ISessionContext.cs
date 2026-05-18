namespace Winterhaven.Gateway.Core.Application.Services.Sessions;

using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

public interface ISessionContext
{
    UserSession? Session { get; }
}