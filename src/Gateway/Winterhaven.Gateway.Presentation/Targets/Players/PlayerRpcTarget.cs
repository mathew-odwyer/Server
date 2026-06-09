using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Winterhaven.Common.DTOs.Players;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Players;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Attributes;

namespace Winterhaven.Gateway.Presentation.Targets.Players;

[ExcludeFromCodeCoverage]
internal sealed record PlayerActionRpcParameters(
    PlayerActionDto[] ActionQueue);

internal sealed class PlayerRpcTarget : IRpcTarget
{
    private readonly IMessageBus messageBus;

    private readonly IUserSessionContext userSessionContext;

    public PlayerRpcTarget(IMessageBus messageBus, IUserSessionContext userSessionContext)
    {
        this.messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("player.action", UseSingleObjectParameterDeserialization = true)]
    public async Task PerformAction(PlayerActionRpcParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var notification = new PlayerActionEvent(parameters.ActionQueue);

        var options = new PublishOptions()
            .WithRouteKey("playerId", userSessionContext.UserSession!.UserAccountId.ToString());

        await messageBus.PublishAsync(notification, options, cancellationToken).ConfigureAwait(false);
    }
}
