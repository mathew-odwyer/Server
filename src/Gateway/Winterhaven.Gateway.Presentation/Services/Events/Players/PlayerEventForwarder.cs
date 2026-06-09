using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Players;
using Winterhaven.Gateway.Core.Application.Services.Users;

namespace Winterhaven.Gateway.Presentation.Services.Events.Players;

internal sealed class PlayerEventForwarder : EventForwarderBase
{
    private readonly IUserSessionContext userSessionContext;

    public PlayerEventForwarder(
        IMessageBus messageBus,
        IUserSessionContext userSessionContext)
        : base(messageBus) =>
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));

    protected override bool CanForward() =>
        base.CanForward() &&
        userSessionContext.IsAuthenticated &&
        userSessionContext.UserSession != null;

    protected override async Task RegisterSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        if (!CanForward())
        {
            throw new InvalidOperationException("Subscriptions must be registered after the user session has been established.");
        }

        string userAccountId = userSessionContext.UserSession!.UserAccountId.ToString();

        var subscribeOptions = new SubscribeOptions()
            .WithRouteKey("playerId", userAccountId);

        await SubscribeAsync<PlayerNotifiedEvent>(OnPlayerNotified, subscribeOptions, cancellationToken).ConfigureAwait(false);
    }

    private async Task OnPlayerNotified(PlayerNotifiedEvent data, CancellationToken cancellationToken)
        => await ForwardAsync(data.Method, data.Params, cancellationToken).ConfigureAwait(false);
}
