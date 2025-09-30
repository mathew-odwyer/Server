// <copyright file="DeletePlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.DeletePlayer;

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

public sealed class DeletePlayerRequestHandler : IRequestHandler<DeletePlayerRequest>
{
    private readonly ILogger<DeletePlayerRequestHandler> logger;

    private readonly IPlayerRepository playerRepository;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    public DeletePlayerRequestHandler(
        ILogger<DeletePlayerRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IPlayerRepository playerRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    public async Task Handle(DeletePlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("Deleting player: '{Name}' for user with ID: '{UserAccountId}'", request.Name, request.UserAccountId);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
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

        player.IsDeleted = true;

        try
        {
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            this.logger.LogError("Failed to delete player: '{Name}' for user with ID: '{UserAccountId}'", request.Name, request.UserAccountId);
            throw new PlayerDeleteException(request.UserAccountId, request.Name, ex);
        }
    }
}
