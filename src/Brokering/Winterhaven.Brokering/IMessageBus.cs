using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Brokering;

/// <summary>
/// </summary>
/// <typeparam name="TData">
/// </typeparam>
/// <param name="data">
/// </param>
public delegate Task MessageConsumer<TData>(TData data)
    where TData : class;

/// <summary>
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TData">
    /// </typeparam>
    /// <param name="data">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    /// </returns>
    public Task PublishAsync<TData>(TData data, CancellationToken cancellationToken = default)
        where TData : class;

    /// <summary>
    /// </summary>
    /// <typeparam name="TData">
    /// </typeparam>
    /// <param name="consumer">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    /// </returns>
    public Task SubscribeAsync<TData>(MessageConsumer<TData> consumer, CancellationToken cancellationToken = default)
        where TData : class;
}
