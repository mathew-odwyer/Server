namespace Winterhaven.Brokering.NATS;

using global::NATS.Client.Core;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Brokering.NATS.Resolving;

internal sealed class NatsEventPublisher : IEventPublisher
{
    private readonly INatsConnection connection;

    private readonly ILogger<NatsEventPublisher> logger;

    private readonly INatsSubjectResolver subjectResolver;

    public NatsEventPublisher(ILogger<NatsEventPublisher> logger, INatsConnection connection, INatsSubjectResolver subjectResolver)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.subjectResolver = subjectResolver ?? throw new ArgumentNullException(nameof(subjectResolver));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishEventAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(e);

        string subject = this.subjectResolver.ResolveSubject<TEvent>();

        this.logger.LogTrace("Publishing NATS Event: {Subject}", subject);

        await this.connection.PublishAsync(
            subject: subject,
            data: e,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}