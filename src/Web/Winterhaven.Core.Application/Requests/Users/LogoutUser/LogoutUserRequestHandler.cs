// <copyright file="LogoutUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.LogoutUser;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Core.Application.Contexts.Users;
using Winterhaven.Core.Application.Exceptions;
using Winterhaven.Core.Application.Exceptions.Database;
using Winterhaven.Core.Application.Work;
using Winterhaven.Core.Application.Work.Users;
using Winterhaven.Core.Domain.Entities.Users;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides a request handler used used to logout a <see cref="UserAccount"/> and invalidate their currently active session.
/// </summary>
public sealed class LogoutUserRequestHandler : IRequestHandler<LogoutUserRequest>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<LogoutUserRequestHandler> logger;

    /// <summary>
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account context, used to fetch the currently authenticated user.
    /// </summary>
    private readonly IUserAccountContext userAccountContext;

    /// <summary>
    /// The user session token repository, used to expire the currently active session.
    /// </summary>
    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory.
    /// </param>
    /// <param name="userSessionTokenRepository">
    /// The user session token repository, used to expire the currently active session.
    /// </param>
    /// <param name="userAccountContext">
    /// The user account context, used to fetch the currently authenticated user.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userSessionTokenRepository"/></description></item>
    ///   <item><description><paramref name="userAccountContext"/></description></item>
    /// </list>
    /// </exception>
    public LogoutUserRequestHandler(
        ILogger<LogoutUserRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository,
        IUserAccountContext userAccountContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
        this.userAccountContext = userAccountContext ?? throw new ArgumentNullException(nameof(userAccountContext));
    }

    /// <inheritdoc/>
    public async Task Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userAccount = this.userAccountContext.User;

        this.logger.LogInformation("User logging out with ID: {UserAccountId}", userAccount.Id);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, cancellationToken).ConfigureAwait(false);

        if (activeSession == null)
        {
            this.logger.LogError("Failed to locate active session for user with ID: '{UserAccountId}'", userAccount.Id);
            throw new InvalidOperationException($"Failed to locate an active session for user with ID: '{userAccount.Id}'");
        }

        try
        {
            activeSession.IsRevoked = true;
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            // Lets keep this here just incase someone attempts to spoof tokens.
            // There's really no need for concurrency handling, but a DB failure will bubble up as a 500 without context.
            this.logger.LogError(ex, "Failed to logout from active session for user with ID: {UserAccountId}", userAccount.Id);
            throw new ConflictException("Logout failed due to a system error.", ex);
        }
    }
}
