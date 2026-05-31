using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal interface IUserTokenParser
{
    public UserSession ParseUserToken(string userToken);
}
