namespace Winterhaven.Gateway.Core.Application.Requests.Users.UserLogin;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Sessions;

public sealed class UserLoginRequestHandler : IRequestHandler<UserLoginRequest, UserLoginResponse>
{
    private readonly ILogger<UserLoginRequestHandler> logger;

    private readonly IUserAccountClient userAccountClient;

    private readonly ISessionAuthenticator sessionAuthenticator;

    public UserLoginRequestHandler(ILogger<UserLoginRequestHandler> logger, IUserAccountClient userAccountClient, ISessionAuthenticator sessionAuthenticator)
    {
        this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
        this.sessionAuthenticator = sessionAuthenticator ?? throw new ArgumentNullException(nameof(sessionAuthenticator));
    }

    public async Task<UserLoginResponse> Handle(UserLoginRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogDebug("Handling user login request for username: {Username}", request.Username);

        var dto = new LoginUserRequestDto()
        {
            Username = request.Username,
            Password = request.Password
        };

        // Attempt to login, if it fails it will bubble up to a 401 and return a JSON-RPC error response with the appropriate message.
        var response = await this.userAccountClient.LoginUserAsync(dto, cancellationToken).ConfigureAwait(false);

        this.logger.LogDebug("User login successful for username: {Username}", request.Username);
        this.logger.LogDebug("Authenticating user session for username: {Username}", request.Username);

        this.sessionAuthenticator.Authenticate(response.AccessToken);

        return new UserLoginResponse(
            RefreshToken: response.RefreshToken);
    }
}
