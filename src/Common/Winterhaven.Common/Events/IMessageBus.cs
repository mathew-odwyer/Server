using System;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Common.Events;

/// <summary>
/// </summary>
public delegate Task MessageConsumer<TData>(TData data, CancellationToken cancellationToken = default)
    where TData : IEvent;

/// <summary>
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// </summary>
    public Task PublishAsync<TData>(
        TData data,
        PublishOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent;

    /// <summary>
    /// </summary>
    public Task<IAsyncDisposable> SubscribeAsync<TData>(
        MessageConsumer<TData> consumer,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent;
}
