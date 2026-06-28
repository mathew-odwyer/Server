using System;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Common.Events;

/// <summary>
/// Represents a callback that handles an event of type <typeparamref name="TData"/> when received from the message bus.
/// </summary>
/// <typeparam name="TData">
/// The event type being consumed.
/// </typeparam>
/// <param name="data">
/// The event payload.
/// </param>
/// <param name="cancellationToken">
/// The token used to cancel handling of the event.
/// </param>
public delegate Task MessageConsumer<in TData>(TData data, CancellationToken cancellationToken = default)
    where TData : IEvent;

/// <summary>
/// Defines an interface that represents a message bus used for dispatching events between decoupled components.
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publishes an event to all subscribed consumers.
    /// </summary>
    /// <typeparam name="TData">
    /// The event type being published.
    /// </typeparam>
    /// <param name="data">
    /// The event payload to publish.
    /// </param>
    /// <param name="options">
    /// Optional settings controlling how the event is published.
    /// </param>
    /// <param name="cancellationToken">
    /// The token used to cancel the publish operation.
    /// </param>
    public Task PublishAsync<TData>(
        TData data,
        PublishOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent;

    /// <summary>
    /// Registers a consumer to receive events of type <typeparamref name="TData"/>.
    /// </summary>
    /// <typeparam name="TData">
    /// The event type to subscribe to.
    /// </typeparam>
    /// <param name="consumer">
    /// The callback invoked for each received event.
    /// </param>
    /// <param name="options">
    /// Optional settings controlling the subscription.
    /// </param>
    /// <param name="cancellationToken">
    /// The token used to cancel the subscribe operation.
    /// </param>
    /// <returns>
    /// A disposable that, when disposed, unsubscribes the consumer.
    /// </returns>
    public Task<IAsyncDisposable> SubscribeAsync<TData>(
        MessageConsumer<TData> consumer,
        SubscribeOptions? options = null,
        CancellationToken cancellationToken = default)
        where TData : IEvent;
    
    /// <summary>
    /// Sends a request and asynchronously waits for a single reply from a responder.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The request type being sent.
    /// </typeparam>
    /// <typeparam name="TReply">
    /// The expected reply type.
    /// </typeparam>
    /// <param name="data">
    /// The request payload to send.
    /// </param>
    /// <param name="options">
    /// Optional settings controlling how the request is sent.
    /// </param>
    /// <param name="cancellationToken">
    /// The token used to cancel the request operation.
    /// </param>
    /// <returns>
    /// The reply received from the responder.
    /// </returns>
    public Task<TReply> RequestAsync<TRequest, TReply>(
        TRequest data,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TReply>
        where TReply : notnull;
}
