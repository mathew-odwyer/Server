using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Winterhaven.Common.Events;
using Winterhaven.Common.Exceptions;

namespace Winterhaven.Events;

[ExcludeFromCodeCoverage]
internal sealed class NatsMessageBus : IMessageBus
{
    private readonly INatsConnection connection;

    private readonly ILogger<NatsMessageBus> logger;

    public NatsMessageBus(ILogger<NatsMessageBus> logger, INatsConnection connection)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task PublishAsync<TData>(
        TData data,
        PublishOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent
    {
        ArgumentNullException.ThrowIfNull(data);

        var publishOptions = options ?? new PublishOptions();
        string subject = TData.GetPublishEventRoute(publishOptions);

        logger.LogTrace("Publishing message of type '{MessageType}' to subject '{Subject}'", typeof(TData).FullName, subject);

        try
        {
            await connection.PublishAsync(
                subject: subject,
                data: data,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (NatsException ex)
        {
            throw new MessageBusException($"Failed to publish event: '{subject}'", ex);
        }
    }

    public async Task<IAsyncDisposable> SubscribeAsync<TData>(
        MessageConsumer<TData> consumer,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent
    {
        ArgumentNullException.ThrowIfNull(consumer);

        var subscribeOptions = options ?? new SubscribeOptions();
        string subject = TData.GetSubscribeEventRoute(subscribeOptions);

        logger.LogTrace("Subscribing to '{MessageType}' on '{Subject}'", typeof(TData).FullName, subject);

        //// Link to the caller's token so either the caller cancelling or DisposeAsync()
        //// can stop the loop. Without this we'd have no owned handle to cancel on disposal.
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        try
        {
            var subscription = await connection
                .SubscribeCoreAsync<TData>(subject, cancellationToken: cts.Token)
                .ConfigureAwait(false);

            var loopTask = Task.Run(async () =>
            {
                // Exceptions are caught per-message so a single bad message doesn't kill the entire subscription. OperationCanceledException is intentionally let through so the loop exits cleanly when the token is cancelled.
                await foreach (var msg in subscription.Msgs.ReadAllAsync(cts.Token).ConfigureAwait(false))
                {
                    if (msg.Data is null) continue;

                    try
                    {
                        await consumer(msg.Data, cts.Token).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        logger.LogError(ex, "Unhandled exception in consumer for subject '{Subject}'", subject);
                    }
                }
            }, cts.Token);

            return new NatsSubscription<TData>(subscription, cts, loopTask, logger);
        }
        catch (MessageBusException ex)
        {
            throw new MessageBusException($"Failed to subscribe to event: '{subject}'", ex);
        }
    }

    private class NatsSubscription<TData> : IAsyncDisposable
        where TData : IEvent
    {
        private readonly CancellationTokenSource cts;

        private readonly ILogger<NatsMessageBus> logger;

        private readonly Task loopTask;

        private readonly INatsSub<TData> subscription;

        public NatsSubscription(
            INatsSub<TData> subscription,
            CancellationTokenSource cts,
            Task loopTask,
            ILogger<NatsMessageBus> logger)
        {
            this.subscription = subscription;
            this.cts = cts;
            this.loopTask = loopTask;
            this.logger = logger;
        }

        public async ValueTask DisposeAsync()
        {
            //// Cancel first, then wait for the loop to fully exit before disposing the NATS subscription.
            //// This avoids a race where the loop tries to read from a disposed channel.
            await cts.CancelAsync().ConfigureAwait(false);

            try
            {
                await loopTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected if the token is cancelled.
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Exception during subscription loop teardown");
                throw;
            }

            await subscription.DisposeAsync().ConfigureAwait(false);
            cts.Dispose();
        }
    }
}
