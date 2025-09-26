// <copyright file="CreatePlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Contexts;
using Web.Application.Contexts.Players;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

public sealed class CreatePlayerRequestHandler : IRequestHandler<CreatePlayerRequest, Result<CreatePlayerResponse>>
{
    private readonly ILogger<CreatePlayerRequestHandler> logger;

    private readonly IPlayerRepository playerRepository;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    private readonly IUserAccountRepository userAccountRepository;

    public CreatePlayerRequestHandler(
        ILogger<CreatePlayerRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IPlayerRepository playerRepository,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    public async Task<Result<CreatePlayerResponse>> Handle(CreatePlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("Creating new player for user with ID: '{UserAccountId}'", request.UserAccountId);
        var userAccount = await this.userAccountRepository.GetByIdAsync(request.UserAccountId, cancellationToken).ConfigureAwait(false);

        if (userAccount == null)
        {
            this.logger.LogError("Failed to locate user with ID: '{UserAccountId}'", request.UserAccountId);
            throw new EntityNotFoundException(nameof(UserAccount), request.UserAccountId);
        }

        bool playerExists = await this.playerRepository.IsPlayerExists(request.Name, cancellationToken).ConfigureAwait(false);

        if (playerExists)
        {
            this.logger.LogInformation("Player already exists with name: '{Name}'", request.Name);
            return Result.Fail($"Player already exists with name: '{request.Name}'");
        }

        var player = new Player()
        {
            UserAccount = userAccount,
            Name = request.Name,
        };

        var work = this.unitOfWorkFactory.CreateUnitOfWork();

        try
        {
            await this.playerRepository.AddAsync(player, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            this.logger.LogError(ex, "Failed to create new player for user with ID: {UserAccountId}", userAccount.Id);
            return Result.Fail("Failed to create new player, please try again in a few moments.");
        }

        this.logger.LogInformation("Player '{Name}' created for user with ID: '{UserAccountId}'", player.Name, request.UserAccountId);

        return Result.Ok(new CreatePlayerResponse()
        {
            Username = player.Name
        });
    }
}
