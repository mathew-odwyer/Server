namespace Winterhaven.Gateway.Presentation.Consumers;

using StreamJsonRpc;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Brokering;
using Winterhaven.Brokering.Events.Players;

internal sealed class PlayerNotificationEventConsumer : IEventConsumer<PlayerNotificationEvent>
{
    private readonly JsonRpc rpc;

    public PlayerNotificationEventConsumer(JsonRpc rpc)
    {
        this.rpc = rpc ?? throw new System.ArgumentNullException(nameof(rpc));
    }

    public async Task OnEventAsync(PlayerNotificationEvent e, CancellationToken cancellationToken)
    {
        if (this.rpc.IsDisposed)
        {
            return;
        }

        await this.rpc.NotifyAsync(e.Method, e.Params).ConfigureAwait(false);
    }
}