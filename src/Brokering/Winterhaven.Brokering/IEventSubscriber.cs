namespace Winterhaven.Brokering;

using System.Threading;
using System.Threading.Tasks;

public interface IEventSubscriber
{
    Task SubscribeAsync<TEvent>(IEventConsumer<TEvent> consumer, CancellationToken cancellationToken = default)
        where TEvent : class;
}