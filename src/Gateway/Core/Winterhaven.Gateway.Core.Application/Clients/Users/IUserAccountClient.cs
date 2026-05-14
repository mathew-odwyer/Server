namespace Winterhaven.Gateway.Core.Application.Clients.Users;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Common.DTOs.Users;

public interface IUserAccountClient
{
    Task<LoginUserResponseDto> LoginUserAsync(LoginUserRequestDto dto, CancellationToken cancellationToken);

    Task RegisterUserAsync(RegisterUserRequestDto dto, CancellationToken cancellationToken);
}
