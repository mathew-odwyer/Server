namespace Winterhaven.Brokering.NATS;

using global::NATS.Client.Core;
using Microsoft.Extensions.Logging;
using Winterhaven.Brokering.NATS.Resolving;

internal sealed class NatsEventSubscriber : IEventSubscriber
{
    private readonly INatsConnection connection;

    private readonly ILogger<NatsEventSubscriber> logger;

    private readonly INatsSubjectResolver subjectResolver;

    public NatsEventSubscriber(ILogger<NatsEventSubscriber> logger, INatsConnection connection, INatsSubjectResolver subjectResolver)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.subjectResolver = subjectResolver ?? throw new ArgumentNullException(nameof(subjectResolver));
    }

    public async Task SubscribeAsync<TEvent>(IEventConsumer<TEvent> consumer, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(consumer);

        string subject = this.subjectResolver.ResolveSubject<TEvent>();

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