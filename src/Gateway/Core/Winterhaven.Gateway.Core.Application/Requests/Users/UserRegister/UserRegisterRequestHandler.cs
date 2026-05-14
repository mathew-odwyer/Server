namespace Winterhaven.Gateway.Core.Application.Requests.Users.UserRegister;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Common.DTOs.Users;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Clients.Users;

public sealed class UserRegisterRequestHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
{
    private readonly ILogger<UserRegisterRequestHandler> logger;

    private readonly IUserAccountClient userAccountClient;

    public UserRegisterRequestHandler(ILogger<UserRegisterRequestHandler> logger, IUserAccountClient userAccountClient)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountClient = userAccountClient ?? throw new ArgumentNullException(nameof(userAccountClient));
    }

    public async Task<UserRegisterResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogDebug("Handling user registration request for username: {Username}", request.Username);

        var dto = new RegisterUserRequestDto()
        {
            Username = request.Username,
            EmailAddress = request.EmailAddress,
            Password = request.Password
        };

        await this.userAccountClient.RegisterUserAsync(dto, cancellationToken).ConfigureAwait(false);
        return new UserRegisterResponse(Success: true);
    }
}
