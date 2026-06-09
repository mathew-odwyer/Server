using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Players;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;

/// <summary>
///   Provides a request handler used to update all a player for the current user account.
/// </summary>
public sealed class UpdatePlayerRequestHandler : IRequestHandler<UpdatePlayerRequest>
{
    /// <summary>
    ///   The logger.
    /// </summary>
    private readonly ILogger<UpdatePlayerRequestHandler> logger;

    /// <summary>
    ///   The player repository, used to fetch the player to update.
    /// </summary>
    private readonly IPlayerRepository playerRepository;

    /// <summary>
    ///   The unit of work factory, used to save the changes of the player to update.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    ///   Initializes a new instance of the <see cref="UpdatePlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    ///   The logger.
    /// </param>
    /// <param name="unitOfWorkFactory">
    ///   The unit of work factory, used to save the changes of the player to update.
    /// </param>
    /// <param name="playerRepository">
    ///   The player repository, used to fetch the player to update.
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
    ///       <description><paramref name="playerRepository"/></description>
    ///     </item>
    ///   </list>
    /// </exception>
    public UpdatePlayerRequestHandler(
        ILogger<UpdatePlayerRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IPlayerRepository playerRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    /// <inheritdoc/>
    public async Task Handle(UpdatePlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var player = await this.playerRepository.GetByIdAsync(request.PlayerId, cancellationToken).ConfigureAwait(false)
            ?? throw new ResourceNotFoundException(nameof(Player), request.PlayerId);

        this.logger.LogDebug("Updating player with ID: '{PlayerId}'", player.Id);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();

        player.X = request.X ?? player.X;
        player.Y = request.Y ?? player.Y;

        await work.SaveAsync(cancellationToken).ConfigureAwait(false);

        this.logger.LogInformation("Player updated with ID: '{PlayerId}'", player.Id);
    }
}
