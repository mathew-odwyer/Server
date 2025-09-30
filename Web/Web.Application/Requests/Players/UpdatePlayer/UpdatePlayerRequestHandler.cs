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

public sealed class UpdatePlayerRequestHandler : IRequestHandler<UpdatePlayerRequest>
{
    private readonly ILogger<UpdatePlayerRequestHandler> logger;

    private readonly IPlayerRepository playerRepository;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    public UpdatePlayerRequestHandler(
        ILogger<UpdatePlayerRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IPlayerRepository playerRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

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
