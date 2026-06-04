using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Brokering;

/// <summary>
///   Represents a delegate that defines the signature for a message consumer, which is responsible for processing messages of a specific type.
/// </summary>
/// <typeparam name="TData">
///   The type of data that the message consumer will process.
/// </typeparam>
/// <param name="data">
///   The message data that the consumer will process.
/// </param>
public delegate Task MessageConsumer<TData>(TData data)
    where TData : class;

/// <summary>
///   Defines an interface that represents a message bus, which is responsible for publishing messages to subscribers and allowing consumers to subscribe to specific message types.
/// </summary>
public interface IMessageBus
{
    /// <summary>
    ///   Publishes a message of a specific type to the message bus, allowing any subscribed consumers to receive and process the message.
    /// </summary>
    /// <typeparam name="TData">
    ///   The type of data that the message being published contains.
    /// </typeparam>
    /// <param name="data">
    ///   The message data that is being published to the message bus. This data will be received and processed by any subscribed consumers that are interested in this type of message.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token that can be used to cancel the publishing operation if needed. This allows for graceful cancellation of the message publishing process, ensuring that any ongoing operations can be stopped if necessary.
    /// </param>
    /// <returns>
    ///   A <see cref="Task"/> that represents the asynchronous operation of publishing the message.
    /// </returns>
    /// <remarks>
    ///   The contract does not guarantee that the message will be delivered to all subscribers, as it is possible for some subscribers to miss messages due to various reasons such as network issues or subscriber downtime. Therefore, it is the responsibility of the implementation to document the delivery guarantees and behavior of the message bus, including any potential limitations or scenarios where messages may not be delivered to subscribers.
    /// </remarks>
    public Task PublishAsync<TData>(TData data, CancellationToken cancellationToken = default)
        where TData : class;

    /// <summary>
    ///   Subscribes a message consumer to the message bus, allowing the consumer to receive and process messages of a specific type when they are published. The consumer will be invoked with the message data whenever a message of the corresponding type is published to the message bus.
    /// </summary>
    /// <typeparam name="TData">
    ///   The type of data that the message consumer will process.
    /// </typeparam>
    /// <param name="consumer">
    ///   The message consumer that will be subscribed to the message bus.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token that can be used to cancel the subscription operation if needed. This allows for graceful cancellation of the subscription process, ensuring that any ongoing operations can be stopped if necessary.
    /// </param>
    /// <returns>
    ///   A <see cref="Task"/> that represents the asynchronous operation of subscribing the message consumer to the message bus. The task will complete when the subscription process is finished, allowing the consumer to start receiving messages of the specified type.
    /// </returns>
    /// <remarks>
    ///   The contract does not guarantee that the consumer will receive all messages of the specified type, as it is possible for some messages to be missed due to various reasons such as network issues or subscriber downtime. Therefore, it is the responsibility of the implementation to document the delivery guarantees and behavior of the message bus, including any potential limitations or scenarios where messages may not be delivered to subscribers.
    /// </remarks>
    public Task SubscribeAsync<TData>(MessageConsumer<TData> consumer, CancellationToken cancellationToken = default)
        where TData : class;
}
