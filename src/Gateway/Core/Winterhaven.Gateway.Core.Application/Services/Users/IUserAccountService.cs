namespace Winterhaven.Gateway.Core.Application.Services.Users;

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed record UserLoginResult(
    string RefreshToken);

public sealed record UserRefreshResult(
    string RefreshToken);

public sealed record UserRegistrationResult(
    bool Success);

public interface IUserAccountService : IDisposable
{
    Task<UserLoginResult> LoginUserAsync(string username, string password, CancellationToken cancellationToken);

    Task LogoutUserAsync(CancellationToken cancellationToken);

    Task<UserRefreshResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

    Task<UserRegistrationResult> RegisterUserAsync(string username, string password, string emailAddress, CancellationToken cancellationToken);
}