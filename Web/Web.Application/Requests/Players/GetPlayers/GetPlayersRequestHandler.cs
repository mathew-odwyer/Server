// <copyright file="GetPlayersRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

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
/// Provides a request handler used to fetch all existing players for the current <see cref="UserAccount"/>.
/// </summary>
public sealed class GetPlayersRequestHandler : IRequestHandler<GetPlayersRequest, GetPlayersResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<GetPlayersRequestHandler> logger;

    /// <summary>
    /// The mapper, used to map between <see cref="Player"/> and <see cref="PlayerDto"/>.
    /// </summary>
    private readonly IMapper mapper;

    /// <summary>
    /// The player repository, used to fetch all players for the current <see cref="UserAccount"/>.
    /// </summary>
    private readonly IPlayerRepository playerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlayersRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="mapper">
    /// The mapper, used to map between <see cref="Player"/> and <see cref="PlayerDto"/>.
    /// </param>
    /// <param name="playerRepository">
    /// The player repository, used to fetch all players for the current <see cref="UserAccount"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="mapper"/></description></item>
    ///   <item><description><paramref name="playerRepository"/></description></item>
    /// </list>
    /// </exception>
    public GetPlayersRequestHandler(
        ILogger<GetPlayersRequestHandler> logger,
        IMapper mapper,
        IPlayerRepository playerRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    /// <inheritdoc/>
    public async Task<GetPlayersResponse> Handle(GetPlayersRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var players = await this.playerRepository.GetPlayersByUserAccountId(request.UserAccountId, cancellationToken).ConfigureAwait(false);

        this.logger.LogInformation("Fetching all playeres for user with ID: '{UserAccountId}'", request.UserAccountId);

        if (players == null || !players.Any())
        {
            this.logger.LogError("Failed to fetch players for user with ID: '{UserAccountId}'", request.UserAccountId);
            throw new EntityNotFoundException(nameof(UserAccount), request.UserAccountId);
        }

        return new GetPlayersResponse(this.mapper.Map<IEnumerable<PlayerDto>>(players));
    }
}
