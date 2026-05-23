namespace Winterhaven.Brokering;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
///   Defines an interface that represents an event subscriber.
/// </summary>
public interface IEventSubscriber
{
    /// <summary>
    ///   Subscribes the specified <paramref name="consumer"/> to the event bus.
    /// </summary>
    /// <typeparam name="TEvent">
    ///   The type of the event.
    /// </typeparam>
    /// <param name="consumer">
    ///   The consumer to be subscribed to the event bus.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token.
    /// </param>
    /// <returns>
    ///   Returns a task that is completed when the consumer has subscribed to the event.
    /// </returns>
    Task SubscribeAsync<TEvent>(IEventConsumer<TEvent> consumer, CancellationToken cancellationToken = default)
        where TEvent : class;
}