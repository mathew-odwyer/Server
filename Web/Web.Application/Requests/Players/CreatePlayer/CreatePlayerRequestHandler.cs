// <copyright file="CreatePlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.CreatePlayer;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Contexts;
using Web.Application.Contexts.Players;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Exceptions.Players;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Provides a request handler used to create a new <see cref="Player"/> for the current <see cref="UserAccount"/>.
/// </summary>
public sealed class CreatePlayerRequestHandler : IRequestHandler<CreatePlayerRequest>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<CreatePlayerRequestHandler> logger;

    /// <summary>
    /// The player repository, used to add a new player to the database.
    /// </summary>
    private readonly IPlayerRepository playerRepository;

    /// <summary>
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account repository, used to find the correct <see cref="UserAccount"/>.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory.
    /// </param>
    /// <param name="playerRepository">
    /// The player repository, used to add a new player to the database.
    /// </param>
    /// <param name="userAccountRepository">
    /// The user account repository, used to find the correct <see cref="UserAccount"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="playerRepository"/></description></item>
    ///   <item><description><paramref name="userAccountRepository"/></description></item>
    /// </list>
    /// </exception>
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

    /// <inheritdoc/>
    public async Task Handle(CreatePlayerRequest request, CancellationToken cancellationToken)
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
            throw new ConflictException($"Player already exists with name: '{request.Name}'");
        }

        var player = new Player()
        {
            UserAccountId = userAccount.Id,
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
            throw new PlayerCreateException(request.UserAccountId, request.Name, ex);
        }

        this.logger.LogInformation("Player '{Name}' created for user with ID: '{UserAccountId}'", player.Name, request.UserAccountId);
    }
}
