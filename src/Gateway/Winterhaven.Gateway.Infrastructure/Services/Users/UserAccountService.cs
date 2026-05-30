using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Users;

namespace Winterhaven.Gateway.Infrastructure.Services.Users;

internal sealed class UserAccountService : IUserAccountService
{
    private readonly ILogger<UserAccountService> logger;

    private readonly IUserAccountClient userAccountClient;

    public UserAccountService(ILogger<UserAccountService> logger, IUserAccountClient userAccountClient)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
    }

    public async Task<UserLoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var dto = new LoginUserRequestDto()
        {
            Username = username,
            Password = password,
        };

        logger.LogInformation("User logging in: '{Username}'", username);
        var response = await userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("User logged in: '{Username}'", username);

        return new UserLoginResult(
            RefreshToken: response.RefreshToken);
    }

    public async Task RegisterAsync(string username, string password, string emailAddress, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);

        var dto = new RegisterUserRequestDto()
        {
            Username = username,
            Password = password,
            EmailAddress = emailAddress,
        };

        logger.LogInformation("Registering potential user: '{Username}'", username);
        await userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Registered potential user: '{Username}'", username);
    }
}
