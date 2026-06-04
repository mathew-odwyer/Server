namespace Winterhaven.Brokering.NATS;

internal sealed class NatsMessageBus : IMessageBus
{
    public Task PublishAsync<TData>(TData data, CancellationToken cancellationToken = default) where TData : class =>
        throw new NotImplementedException();

    public Task SubscribeAsync<TData>(MessageConsumer<TData> consumer, CancellationToken cancellationToken = default) where TData : class =>
        throw new NotImplementedException();
}
