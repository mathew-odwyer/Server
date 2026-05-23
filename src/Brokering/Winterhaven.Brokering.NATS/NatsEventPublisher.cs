namespace Winterhaven.Brokering.NATS;

using global::NATS.Client.Core;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Brokering.NATS.Resolving;

internal sealed class NatsEventPublisher : IEventPublisher
{
    private readonly INatsConnection connection;

    private readonly INatsSubjectResolver subjectResolver;

    public NatsEventPublisher(INatsConnection connection, INatsSubjectResolver subjectResolver)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.subjectResolver = subjectResolver ?? throw new ArgumentNullException(nameof(subjectResolver));
    }

    public async Task PublishEventAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(e);

        await this.connection.PublishAsync(
            subject: this.subjectResolver.ResolveSubject<TEvent>(),
            data: e,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}