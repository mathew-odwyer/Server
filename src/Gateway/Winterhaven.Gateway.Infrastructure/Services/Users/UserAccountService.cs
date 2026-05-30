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

    public async Task<UserRegistrationResult> RegisterAsync(string username, string password, string emailAddress, CancellationToken cancellationToken = default)
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

        //// TODO: Fix this, this is a code smell
        ////       We will always succeed unless an exception is thrown.
        //// TODO: Figure out whether StreamJsonRpc returns anything if the return type is void in UserRpcTarget.
        return new UserRegistrationResult(
            Success: true);
    }
}
