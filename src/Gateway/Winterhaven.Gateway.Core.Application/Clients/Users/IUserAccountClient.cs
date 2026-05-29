using System.Threading;
using System.Threading.Tasks;
using Refit;
using Winterhaven.Common.DTOs.Users;

namespace Winterhaven.Gateway.Core.Application.Clients.Users;

/// <summary>
/// </summary>
public interface IUserAccountClient
{
    /// <summary>
    /// </summary>
    /// <param name="dto">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    [Post("/Login")]
    public Task<LoginUserResponseDto> LoginUserAsync([Body] LoginUserRequestDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="cancellationToken">
    /// </param>
    [Post("/Logout")]
    public Task LogoutUserAsync(CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="dto">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    [Post("/RefreshToken")]
    public Task<RefreshTokenResponseDto> RefreshTokenAsync([Body] RefreshTokenRequestDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// </summary>
    /// <param name="dto">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    [Post("/Register")]
    public Task RegisterUserAsync([Body] RegisterUserRequestDto dto, CancellationToken cancellationToken);
}
