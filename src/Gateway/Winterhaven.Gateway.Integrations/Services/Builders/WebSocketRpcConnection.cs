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

        this.formatter = new SystemTextJsonFormatter()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                ////UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            }
        };

        this.messageHandler = new WebSocketMessageHandler(this.webSocket, this.formatter);
        this.jsonRpc = new JsonRpc(this.messageHandler);

        this.typeToProxyMap = [];

        var proxyOptions = new JsonRpcProxyOptions()
        {
            ServerRequiresNamedArguments = true,
        };

        foreach (var type in proxyTypes)
        {
            this.typeToProxyMap[type] = this.jsonRpc.Attach(type, proxyOptions);
        }

        this.jsonRpc.StartListening();
    }

    public WebSocketState State
    {
        get
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, nameof(WebSocketRpcConnection));
            return this.webSocket.State;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (this.isDisposed)
        {
            return;
        }

        if (this.jsonRpc != null)
        {
            this.jsonRpc.Dispose();
            this.jsonRpc = null;
        }

        if (this.messageHandler != null)
        {
            await this.messageHandler.DisposeAsync().ConfigureAwait(false);
            this.messageHandler = null;
        }

        if (this.formatter != null)
        {
            this.formatter.Dispose();
            this.formatter = null;
        }

        if (this.typeToProxyMap != null)
        {
            this.typeToProxyMap.Clear();
            this.typeToProxyMap = null;
        }

        if (this.webSocket != null)
        {
            this.webSocket.Dispose();
            this.webSocket = null;
        }

        this.isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public TProxy GetProxy<TProxy>()
                where TProxy : class
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, nameof(WebSocketRpcConnection));

        var type = typeof(TProxy);

        return !this.typeToProxyMap.TryGetValue(type, out object proxy)
            ? throw new ArgumentException($"Failed to fetch proxy for type: {type.FullName}")
            : (TProxy)proxy;
    }
}
