// <copyright file="GetPlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Contexts;
using Web.Application.DTOs.Players;
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
    /// The user account context, used to fetch the currently authenticated user.
    /// </summary>
    private readonly IUserAccountContext userAccountContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="mapper">
    /// The mapper, used to map a <see cref="Player"/> to a <see cref="PlayerDto"/>.
    /// </param>
    /// <param name="userAccountContext">
    /// The user account context, used to fetch the currently authenticated user.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="mapper"/></description></item>
    ///   <item><description><paramref name="userAccountContext"/></description></item>
    /// </list>
    /// </exception>
    public GetPlayerRequestHandler(
        ILogger<GetPlayerRequestHandler> logger,
        IMapper mapper,
        IUserAccountContext userAccountContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.userAccountContext = userAccountContext ?? throw new ArgumentNullException(nameof(userAccountContext));
    }

    /// <inheritdoc/>
    public async Task<GetPlayerResponse> Handle(GetPlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userAccount = this.userAccountContext.User;
        var player = userAccount!.Player;

        this.logger.LogInformation("Fetching player with ID: '{PlayerId}'", player.Id);

        return new GetPlayerResponse(this.mapper.Map<PlayerDto>(player));
    }
}
