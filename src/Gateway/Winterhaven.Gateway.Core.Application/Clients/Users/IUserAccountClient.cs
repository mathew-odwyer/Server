using System.Threading;
using System.Threading.Tasks;
using Refit;
using Winterhaven.Common.DTOs.Users;

namespace Winterhaven.Gateway.Core.Application.Clients.Users;

/// <summary>
///   Defines an interface for managing user account authentication and registration operations.
/// </summary>
public interface IUserAccountClient
{
    /// <summary>
    ///   Authenticates a user using the provided login credentials.
    /// </summary>
    /// <param name="dto">
    ///   The login request containing the user's authentication credentials.
    /// </param>
    /// <param name="cancellationToken">
    ///   A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///   A task representing the asynchronous operation, containing the authenticated user response.
    /// </returns>
    [Post("/Login")]
    public Task<LoginUserResponseDto> LoginUserAsync([Body] LoginUserRequestDto dto, CancellationToken cancellationToken);

    /// <summary>
    ///   Logs out the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">
    ///   A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///   A task representing the asynchronous logout operation.
    /// </returns>
    [Post("/Logout")]
    [Headers("Authorization: Bearer")]
    public Task LogoutUserAsync(CancellationToken cancellationToken);

    /// <summary>
    ///   Refreshes the authentication token using the provided refresh token request.
    /// </summary>
    /// <param name="dto">
    ///   The refresh token request containing the current refresh token information.
    /// </param>
    /// <param name="cancellationToken">
    ///   A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///   A task representing the asynchronous operation, containing the refreshed token response.
    /// </returns>
    [Post("/RefreshToken")]
    public Task<RefreshTokenResponseDto> RefreshTokenAsync([Body] RefreshTokenRequestDto dto, CancellationToken cancellationToken);

    /// <summary>
    ///   Registers a new user account using the provided registration details.
    /// </summary>
    /// <param name="dto">
    ///   The registration request containing the new user's account information.
    /// </param>
    /// <param name="cancellationToken">
    ///   A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///   A task representing the asynchronous user registration operation.
    /// </returns>
    [Post("/Register")]
    public Task RegisterUserAsync([Body] RegisterUserRequestDto dto, CancellationToken cancellationToken);
}
