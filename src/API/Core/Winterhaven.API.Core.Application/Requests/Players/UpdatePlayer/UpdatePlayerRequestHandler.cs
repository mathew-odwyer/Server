namespace Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.Common.Exceptions;

/// <summary>
/// Provides a request handler used to update all a <see cref="Player"/> for the current <see cref="UserAccount"/>.
/// </summary>
public sealed class UpdatePlayerRequestHandler : IRequestHandler<UpdatePlayerRequest>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<UpdatePlayerRequestHandler> logger;

    /// <summary>
    /// The unit of work factory, used to save the changes of the <see cref="Player"/> to update.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The actor context, used to fetch the currently authenticated actor.
    /// </summary>
    private readonly IActorContext actorContext;

    /// <summary>
    /// The user account repository, used to fetch the user account (if any) linked to the actor.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory, used to save the changes of the <see cref="Player"/> to update.
    /// </param>
    /// <param name="actorContext">
    /// The user account context, used to fetch the currently authenticated user.
    /// </param>
    /// <param name="userAccountRepository">
    /// The user account repository, used to fetch the user account (if any) linked to the actor.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="logger"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="unitOfWorkFactory"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="actorContext"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="userAccountRepository"/></description>
    /// </item>
    /// </list>
    /// </exception>
    public UpdatePlayerRequestHandler(
        ILogger<UpdatePlayerRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IActorContext actorContext,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.actorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    /// <inheritdoc/>
    public async Task Handle(UpdatePlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var actor = this.actorContext.Actor;
        var userAccount = await this.userAccountRepository.GetByIdAsync(actor.Id, cancellationToken).ConfigureAwait(false)
            ?? throw new ResourceNotFoundException(nameof(UserAccount), actor.Id);
        var player = userAccount!.Player;

        this.logger.LogInformation("Updating player with ID: '{PlayerId}'", player.Id);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();

        player.X = request.X ?? player.X;
        player.Y = request.Y ?? player.Y;

        await work.SaveAsync(cancellationToken).ConfigureAwait(false);
    }
}