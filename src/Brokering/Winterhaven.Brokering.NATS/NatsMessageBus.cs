using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Brokering.NATS;

// TODO: Update UserRpcTargetIntegrationTests to check that the NATS message bus is actually working and delivering messages to subscribers, instead of just being a no-op implementation. This will help ensure that the NATS integration is functioning correctly and that messages are being published and consumed as expected.
internal sealed class NatsMessageBus : IMessageBus
{
    public Task PublishAsync<TData>(TData data, CancellationToken cancellationToken = default) where TData : class => Task.CompletedTask;

    public Task SubscribeAsync<TData>(MessageConsumer<TData> consumer, CancellationToken cancellationToken = default) where TData : class => Task.CompletedTask;
}
