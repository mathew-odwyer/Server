namespace Winterhaven.Brokering;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
///   Defines an interface that represents an event publisher.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    ///   Publishes the specified <typeparamref name="TEvent"/> asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">
    ///   The type of the event to be published.
    /// </typeparam>
    /// <param name="e">
    ///   The event to be published.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token.
    /// </param>
    /// <returns>
    ///   Returns a <see cref="Task"/> that represents the result of the operation.
    /// </returns>
    Task PublishEventAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
        where TEvent : class;
}