using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Services.Events;

internal interface IEventForwarderCoordinator
{
    public Task StartAllForwardersAsync(JsonRpc rpc, CancellationToken cancellationToken = default);
}
