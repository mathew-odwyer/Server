using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Integrations.Services.Builders;

internal sealed class WebSocketRpcConnection : IAsyncDisposable
{
    private SystemTextJsonFormatter formatter;

    private bool isDisposed;

    private JsonRpc jsonRpc;

    private WebSocketMessageHandler messageHandler;

    private Dictionary<Type, object> typeToProxyMap;

    private WebSocket webSocket;

    public WebSocketRpcConnection(WebSocket webSocket, IReadOnlyCollection<Type> proxyTypes)
    {
        this.webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        ArgumentNullException.ThrowIfNull(proxyTypes);

        formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                ////UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            }
        };

        messageHandler = new WebSocketMessageHandler(this.webSocket, formatter);
        jsonRpc = new JsonRpc(messageHandler);

        typeToProxyMap = [];

        var proxyOptions = new JsonRpcProxyOptions()
        {
            ServerRequiresNamedArguments = true,
        };

        foreach (var type in proxyTypes)
        {
            typeToProxyMap[type] = jsonRpc.Attach(type, proxyOptions);
        }

        jsonRpc.StartListening();
    }

    public WebSocketState State
    {
        get
        {
            ObjectDisposedException.ThrowIf(isDisposed, nameof(WebSocketRpcConnection));
            return webSocket.State;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (isDisposed)
        {
            return;
        }

        if (jsonRpc != null)
        {
            jsonRpc.Dispose();
            jsonRpc = null;
        }

        if (messageHandler != null)
        {
            await messageHandler.DisposeAsync().ConfigureAwait(false);
            messageHandler = null;
        }

        if (formatter != null)
        {
            formatter.Dispose();
            formatter = null;
        }

        if (typeToProxyMap != null)
        {
            typeToProxyMap.Clear();
            typeToProxyMap = null;
        }

        if (webSocket != null)
        {
            webSocket.Dispose();
            webSocket = null;
        }

        isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public TProxy GetProxy<TProxy>()
                where TProxy : class
    {
        ObjectDisposedException.ThrowIf(isDisposed, nameof(WebSocketRpcConnection));

        var type = typeof(TProxy);

        return !typeToProxyMap.TryGetValue(type, out object proxy)
            ? throw new ArgumentException($"Failed to fetch proxy for type: {type.FullName}")
            : (TProxy)proxy;
    }
}
