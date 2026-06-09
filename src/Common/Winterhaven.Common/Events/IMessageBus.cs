using System;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Common.Events;

/// <summary>
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <param name="data"></param>
/// <param name="cancellationToken"></param>
public delegate Task MessageConsumer<TData>(TData data, CancellationToken cancellationToken = default)
    where TData : IEvent;

/// <summary>
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="data"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    public Task PublishAsync<TData>(
        TData data,
        PublishOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent;

    /// <summary>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="consumer"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    public Task<IAsyncDisposable> SubscribeAsync<TData>(
        MessageConsumer<TData> consumer,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent;
}
