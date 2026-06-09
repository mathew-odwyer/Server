using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;
using Winterhaven.Common.Events;

namespace Winterhaven.Gateway.Presentation.Services.Events;

internal abstract class EventForwarderBase : IAsyncDisposable
{
    private readonly IMessageBus messageBus;

    private readonly List<IAsyncDisposable> subscriptions;

    private int isDisposed;

    private JsonRpc? rpc;

    protected EventForwarderBase(IMessageBus messageBus)
    {
        this.messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        this.subscriptions = [];
    }

    protected virtual bool IsDisposed
    {
        get
        {
            return this.isDisposed != 0;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!this.TryMarkDisposed())
        {
            return;
        }

        await this.DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(JsonRpc rpc, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(this.IsDisposed, this);
        ArgumentNullException.ThrowIfNull(rpc);

        this.rpc = rpc;

        // Register all subscriptions right away.
        await this.RegisterSubscriptionsAsync(cancellationToken).ConfigureAwait(false);
    }

    protected virtual bool CanForward()
    {
        return !this.IsDisposed && this.rpc is { IsDisposed: false };
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (var subscription in this.subscriptions)
        {
            await subscription.DisposeAsync().ConfigureAwait(false);
        }

        this.subscriptions.Clear();
    }

    protected async Task ForwardAsync(string procedure, object? data, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.IsDisposed, this);

        if (!this.CanForward())
        {
            // If we can't forward the request, assume the task has completed.
            return;
        }

        //// Forward the message and complete either when the message has been sent
        //// or when the caller has requested a cancellation.
        await this.rpc!
            .NotifyWithParameterObjectAsync(procedure, data)
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    protected abstract Task RegisterSubscriptionsAsync(CancellationToken cancellationToken = default);

    protected async Task SubscribeAsync<TData>(
        MessageConsumer<TData> consumer,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
            where TData : IEvent
    {
        ObjectDisposedException.ThrowIf(this.IsDisposed, this);
        ArgumentNullException.ThrowIfNull(consumer);

        var subscription = await this.messageBus.SubscribeAsync(
            consumer: consumer,
            options: options,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (this.IsDisposed)
        {
            // If by the time we've subscribed we're disposed, just dispose of the subscription.
            await subscription.DisposeAsync().ConfigureAwait(false);
            return;
        }

        this.subscriptions.Add(subscription);
    }

    private bool TryMarkDisposed()
    {
        //// Interlocked.Exchange atomically sets disposed to 1
        //// and returns the previous value; returning true means this caller "won" the race.
        return Interlocked.Exchange(ref this.isDisposed, 1) == 0;
    }
}
