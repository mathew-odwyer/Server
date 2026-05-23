namespace Winterhaven.Brokering;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
///   Defines an interface that represents an event consumer.
/// </summary>
/// <typeparam name="TEvent">
///   The type of the event.
/// </typeparam>
public interface IEventConsumer<TEvent>
    where TEvent : class
{
    /// <summary>
    ///   Called when the event is being handled.
    /// </summary>
    /// <param name="e">
    ///   The event.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token.
    /// </param>
    /// <returns>
    ///   Returns a task that is completed when the event has been handled.
    /// </returns>
    Task OnEventAsync(TEvent e, CancellationToken cancellationToken);
}