// <copyright file="UpdatePlayerRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Players.UpdatePlayer;

using Winterhaven.Core.Application.Contexts.Users;
using Winterhaven.Core.Application.Exceptions.Database;
using Winterhaven.Core.Application.Exceptions.Players;
using Winterhaven.Core.Application.Work;
using Winterhaven.Core.Domain.Entities.Players;
using Winterhaven.Core.Domain.Entities.Users;
using MediatR;
using Microsoft.Extensions.Logging;

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
    /// The unit of work factory, used to save the changes of the <see cref="Player"/> to update.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account context, used to fetch the currently authenticated user.
    /// </summary>
    private readonly IUserAccountContext userAccountContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePlayerRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory, used to save the changes of the <see cref="Player"/> to update.
    /// </param>
    /// <param name="userAccountContext">
    /// The user account context, used to fetch the currently authenticated user.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userAccountContext"/></description></item>
    /// </list>
    /// </exception>
    public UpdatePlayerRequestHandler(
        ILogger<UpdatePlayerRequestHandler> logger,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountContext userAccountContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userAccountContext = userAccountContext ?? throw new ArgumentNullException(nameof(userAccountContext));
    }

    /// <inheritdoc/>
    public async Task Handle(UpdatePlayerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userAccount = this.userAccountContext.User;
        var player = userAccount!.Player;

        this.logger.LogInformation("Updating player with ID: '{PlayerId}'", player.Id);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();

        player.X = request.X ?? player.X;
        player.Y = request.Y ?? player.Y;

        try
        {
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            this.logger.LogError(ex, "Failed to save player with ID: '{PlayerId}'", player.Id);
            throw new PlayerUpdateException(player.Id, player.Name, ex);
        }
    }
}
