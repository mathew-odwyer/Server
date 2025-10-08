// <copyright file="UpdatePlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.UpdatePlayer;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Contexts;
using Web.Application.Contexts.Players;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Exceptions.Players;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

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
    /// The player repository, used to fetch the <see cref="Player"/> to update.
    /// </summary>
    private readonly IPlayerRepository playerRepository;

    /// <summary>
    /// The unit of work factory, used to save the changes of the <see cref="Player"/> to update.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory, used to save the changes of the <see cref="Player"/> to update.
    /// </param>
    /// <param name="playerRepository">
    /// The player repository, used to fetch the <see cref="Player"/> to update.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="playerRepository"/></description></item>
    /// </list>
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

        this.logger.LogInformation("Updating player '{Name}' for user with ID: '{UserAccountId}'", request.Name, request.UserAccountId);

        var player = await this.playerRepository.GetPlayerByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);

        if (player == null)
        {
            this.logger.LogError("Player not found with name: '{Name}' for user with ID: '{UserAccountId}'", request.Name, request.UserAccountId);
            throw new EntityNotFoundException(nameof(Player), request.Name);
        }

        if (player.UserAccountId != request.UserAccountId)
        {
            this.logger.LogError("Player found with name: '{Name}' but is not associated with user: '{UserAccountId}'", request.Name, request.UserAccountId);
            throw new ForbiddenAccessException($"Player found with name: '{request.Name}' but is not associated with user: '{request.UserAccountId}'");
        }

        var work = this.unitOfWorkFactory.CreateUnitOfWork();

        player.X = request.X ?? player.X;
        player.Y = request.Y ?? player.Y;

        try
        {
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            this.logger.LogError(ex, "Failed to save player: '{Name}' for user with ID: '{UserAccountId}'", player.Name, player.UserAccountId);
            throw new PlayerUpdateException(player.UserAccountId, player.Name, ex);
        }
    }
}
