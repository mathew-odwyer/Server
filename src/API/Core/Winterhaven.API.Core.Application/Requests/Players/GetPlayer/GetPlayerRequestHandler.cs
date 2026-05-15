namespace Winterhaven.API.Core.Application.Requests.Players.GetPlayer;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

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
    /// The actor context, used to fetch the currently authenticated actor.
    /// </summary>
    private readonly IActorContext actorContext;

    /// <summary>
    /// The user account repository, used to fetch the user account (if any) linked to the actor.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="actorContext">
    /// The actor context, used to fetch the currently authenticated actor.
    /// </param>
    /// <param name="userAccountRepository">
    /// The user account repository, used to fetch the user account (if any) linked to the actor.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="logger"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="actorContext"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="userAccountRepository"/></description>
    /// </item>
    /// </list>
    /// </exception>
    public GetPlayerRequestHandler(
        ILogger<GetPlayerRequestHandler> logger,
        IActorContext actorContext,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.actorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    /// <inheritdoc/>
    public async Task<GetPlayerResponse> Handle(GetPlayerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var actor = this.actorContext.Actor;
        var userAccount = await this.userAccountRepository.GetByIdAsync(actor.Id, cancellationToken).ConfigureAwait(false)
            ?? throw new ResourceNotFoundException(nameof(UserAccount), actor.Id);

        var player = userAccount.Player;

        this.logger.LogDebug("Fetching player with ID: '{PlayerId}'", player.Id);

        return new GetPlayerResponse(
            Name: player.Name,
            X: player.X,
            Y: player.Y);
    }
}