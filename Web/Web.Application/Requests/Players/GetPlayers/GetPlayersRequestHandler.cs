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
using Web.Domain.Entities.Users;

public sealed class GetPlayersRequestHandler : IRequestHandler<GetPlayersRequest, GetPlayersResponse>
{
    private readonly ILogger<GetPlayersRequestHandler> logger;

    private readonly IMapper mapper;

    private readonly IPlayerRepository playerRepository;

    public GetPlayersRequestHandler(
        ILogger<GetPlayersRequestHandler> logger,
        IMapper mapper,
        IPlayerRepository playerRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

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
