namespace Winterhaven.Gateway.Presentation.Targets.Players;

using StreamJsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Brokering;
using Winterhaven.Gateway.Core.Application.Services.Sessions;
using Winterhaven.Gateway.Presentation.Attributes;

internal sealed class PlayerRpcTarget : RpcTargetBase
{
    private readonly IEventPublisher eventPublisher;

    private readonly ISessionContext sessionContext;

    public PlayerRpcTarget(IEventPublisher eventPublisher, ISessionContext sessionContext)
    {
        this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        this.sessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("player.action", UseSingleObjectParameterDeserialization = true)]
    public async Task PeformAction(object parameters, CancellationToken cancellationToken = default)
    {
        await this.eventPublisher.PublishEventAsync($"player.{this.sessionContext.Session!.UserAccountId}.action", parameters, cancellationToken).ConfigureAwait(false);
    }
}