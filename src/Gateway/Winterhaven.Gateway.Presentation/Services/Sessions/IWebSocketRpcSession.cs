using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Gateway.Presentation.Services.Sessions;

internal interface IWebSocketRpcSession
{
    public Task RunAsync(WebSocket socket, CancellationToken cancellationToken = default);
}
