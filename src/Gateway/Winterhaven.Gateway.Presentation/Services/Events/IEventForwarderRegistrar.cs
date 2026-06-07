using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Services.Events;

internal interface IEventForwarderRegistrar
{
    public Task RegisterForwardersAsync(JsonRpc rpc, CancellationToken cancellationToken = default);
}
