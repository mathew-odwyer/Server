// <copyright file="GetPlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Contexts.Players;
using Web.Application.DTOs.Players;
using Web.Application.Exceptions;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Provides a request handler used to fetch an existing <see cref="Player"/> from the current <see cref="UserAccount"/>.
/// </summary>
public sealed class GetPlayerRequestHandler : IRequestHandler<GetPlayerRequest, GetPlayerResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<GetPlayerRequestHandler> logger;

    /// <summary>
    /// The mapper, used to map a <see cref="Player"/> to a <see cref="PlayerDto"/>.
    /// </summary>
    private readonly IMapper mapper;

    /// <summary>
    /// The player repository, used to fetch the <see cref="Player"/>.
    /// </summary>
    private readonly IPlayerRepository playerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="mapper">
    /// The mapper, used to map a <see cref="Player"/> to a <see cref="PlayerDto"/>.
    /// </param>
    /// <param name="playerRepository">
    /// The player repository, used to fetch the <see cref="Player"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="mapper"/></description></item>
    ///   <item><description><paramref name="playerRepository"/></description></item>
    /// </list>
    /// </exception>
    public GetPlayerRequestHandler(
        ILogger<GetPlayerRequestHandler> logger,
        IMapper mapper,
        IPlayerRepository playerRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    /// <inheritdoc/>
    public async Task<GetPlayerResponse> Handle(GetPlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("Fetching player with name: '{Name}'", request.Name);

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

        return new GetPlayerResponse(this.mapper.Map<PlayerDto>(player));
    }
}
