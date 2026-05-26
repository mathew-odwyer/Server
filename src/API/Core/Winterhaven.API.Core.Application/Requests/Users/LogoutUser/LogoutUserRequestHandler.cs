using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Core.Application.Requests.Users.LogoutUser;

/// <summary>
///   Provides a request handler used to logout a user account and invalidate their currently active session.
/// </summary>
public sealed class LogoutUserRequestHandler : IRequestHandler<LogoutUserRequest>
{
    /// <summary>
    ///   The actor context, used to fetch the currently authenticated actor.
    /// </summary>
    private readonly IActorContext actorContext;

    /// <summary>
    ///   The logger.
    /// </summary>
    private readonly ILogger<LogoutUserRequestHandler> logger;

    /// <summary>
    ///   The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    ///   The user account repository, used to fetch the user account (if any) linked to the actor.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    ///   The user session token repository, used to expire the currently active session.
    /// </summary>
    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    /// <summary>
    ///   Initializes a new instance of the <see cref="LogoutUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    ///   The logger.
    /// </param>
    /// <param name="unitOfWorkFactory">
    ///   The unit of work factory.
    /// </param>
    /// <param name="userSessionTokenRepository">
    ///   The user session token repository, used to expire the currently active session.
    /// </param>
    /// <param name="actorContext">
    ///   The actor context, used to fetch the current actor.
    /// </param>
    /// <param name="userAccountRepository">
    ///   The user account repository, used to fetch the user associatd with the actor.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when one of the following parameters is <c>null</c>:
    ///   <list type="bullet">
    ///     <item>
    ///       <description><paramref name="logger"/></description>
    ///     </item>
    ///     <item>
    ///       <description><paramref name="unitOfWorkFactory"/></description>
    ///     </item>
    ///     <item>
    ///       <description><paramref name="userSessionTokenRepository"/></description>
    ///     </item>
    ///     <item>
    ///       <description><paramref name="actorContext"/></description>
    ///     </item>
    ///     <item>
    ///       <description><paramref name="userAccountRepository"/></description>
    ///     </item>
    ///   </list>
    /// </exception>
    public LogoutUserRequestHandler(
        ILogger<LogoutUserRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository,
        IActorContext actorContext,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
        this.actorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    /// <inheritdoc/>
    public async Task Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var actor = actorContext.Actor;
        var userAccount = await userAccountRepository.GetByIdAsync(actor.Id, cancellationToken).ConfigureAwait(false)
            ?? throw new ResourceNotFoundException(nameof(UserAccount), actor.Id);

        logger.LogInformation("User logging out with ID: {UserAccountId}", userAccount.Id);

        var work = unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, cancellationToken).ConfigureAwait(false);

        if (activeSession == null)
        {
            logger.LogError("Failed to locate active session for user with ID: '{UserAccountId}'", userAccount.Id);
            throw new InvalidOperationException($"Failed to locate an active session for user with ID: '{userAccount.Id}'");
        }

        try
        {
            activeSession.IsRevoked = true;
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (EntityPersistenceException ex)
        {
            // Lets keep this here just in-case someone attempts to spoof tokens. There's really no need for concurrency handling, but a DB failure will bubble up as a 500 without context.
            logger.LogError(ex, "Failed to logout from active session for user with ID: {UserAccountId}", userAccount.Id);
            throw new ConflictException("Logout failed due to a system error.", ex);
        }
    }
}