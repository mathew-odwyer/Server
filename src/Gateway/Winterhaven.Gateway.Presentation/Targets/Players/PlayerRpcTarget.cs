using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Winterhaven.Brokering;
using Winterhaven.Brokering.Events.Players;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Attributes;

namespace Winterhaven.Gateway.Presentation.Targets.Players;

internal sealed record PlayerActionData(
    string Type,
    double MoveX,
    double MoveY,
    double Identifier);

internal sealed record PlayerActionRpcParameters(
    PlayerActionData[] ActionQueue);

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
        var options = new PublishOptions()
            .WithRouteKey("playerId", userSessionContext.UserSession!.UserAccountId.ToString());

        var notification = new PlayerActionEvent(
            [.. parameters.ActionQueue
                .Select(x => new PlayerActionEventData(
                    x.Type,
                    x.MoveX,
                    x.MoveY,
                    x.Identifier))]);

        await messageBus.PublishAsync(notification, options, cancellationToken).ConfigureAwait(false);
    }
}
