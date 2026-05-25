namespace Winterhaven.Brokering.NATS;

using global::NATS.Client.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

internal sealed class NatsEventSubscriber : IEventSubscriber
{
    private readonly INatsConnection connection;

    private readonly ILogger<NatsEventSubscriber> logger;

    public NatsEventSubscriber(ILogger<NatsEventSubscriber> logger, INatsConnection connection)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task SubscribeAsync<TEvent>(string subject, IEventConsumer<TEvent> consumer, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(consumer);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        this.logger.LogTrace("Subscribing to NATS Event: {Subject}", subject);

        await foreach (var msg in this.connection.SubscribeAsync<TEvent>(subject, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            if (msg.Data == null)
            {
                continue;
            }

            await consumer.OnEventAsync(msg.Data, cancellationToken).ConfigureAwait(false);
        }
    }
}