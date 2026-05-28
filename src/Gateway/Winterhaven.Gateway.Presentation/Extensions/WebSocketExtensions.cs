using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Gateway.Presentation.Extensions;

[ExcludeFromCodeCoverage]
internal static class WebSocketExtensions
{
    public static async Task SafeCloseAsync(this WebSocket socket, WebSocketCloseStatus status, string description, CancellationToken ct)
    {
        try
        {
            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(status, description, ct).ConfigureAwait(false);
            }
        }
        catch (WebSocketException)
        {
            // connection already broken.
        }
        catch (ObjectDisposedException)
        {
            // already disposed
        }
        catch (InvalidOperationException)
        {
            // invalid state during race conditions
        }
    }
}
