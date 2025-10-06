// <copyright file="LogoutUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;

public sealed class LogoutUserRequestHandler : IRequestHandler<LogoutUserRequest>
{
    private readonly ILogger<LogoutUserRequestHandler> logger;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    public LogoutUserRequestHandler(
        ILogger<LogoutUserRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
    }

    public async Task Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("User logging out with ID: {UserAccountId}", request.UserAccountId);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(request.UserAccountId, cancellationToken).ConfigureAwait(false);

        if (activeSession == null)
        {
            this.logger.LogError("Failed to locate active session for user with ID: '{UserAccountId}'", request.UserAccountId);
            throw new InvalidOperationException($"Failed to locate an active session for user with ID: '{request.UserAccountId}'");
        }

        try
        {
            activeSession.ExpirationDate = DateTime.UtcNow;
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateConcurrencyException ex)
        {
            // Lets keep this here just incase someone attempts to spoof tokens.
            // There's really no need for concurrency handling, but a DB failure will bubble up as a 500 without context.
            this.logger.LogError(ex, "Failed to logout from active session for user with ID: {UserAccountId}", activeSession.UserAccountId);
            throw new UnauthorizedException("Logout failed due to a system error.", ex);
        }
    }
}
