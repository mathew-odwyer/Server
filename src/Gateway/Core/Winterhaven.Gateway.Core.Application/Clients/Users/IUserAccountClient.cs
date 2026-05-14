namespace Winterhaven.Gateway.Core.Application.Clients.Users;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Common.DTOs.Users;

public interface IUserAccountClient
{
    Task RegisterUserAsync(RegisterUserRequestDto dto, CancellationToken cancellationToken);
}
