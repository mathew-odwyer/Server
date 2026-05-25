namespace Winterhaven.Brokering.NATS;

using global::NATS.Client.Core;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

internal sealed class NatsEventPublisher : IEventPublisher
{
    private readonly INatsConnection connection;

    private readonly ILogger<NatsEventPublisher> logger;

    public NatsEventPublisher(ILogger<NatsEventPublisher> logger, INatsConnection connection)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishEventAsync<TEvent>(string subject, TEvent e, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        this.logger.LogTrace("Publishing NATS Event: {Subject}", subject);

        await this.connection.PublishAsync(
            subject: subject,
            data: e,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}