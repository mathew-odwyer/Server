namespace Winterhaven.Brokering;

using System.Threading;
using System.Threading.Tasks;

public interface IEventPublisher
{
    Task PublishEventAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
        where TEvent : class;
}