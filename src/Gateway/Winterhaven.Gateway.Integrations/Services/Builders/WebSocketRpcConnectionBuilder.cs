using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Winterhaven.Gateway.Integrations.Services.Builders;

internal class WebSocketRpcConnectionBuilder
{
    private readonly List<Type> proxyTypes;

    public WebSocketRpcConnectionBuilder() => proxyTypes = [];

    public WebSocketRpcConnection Build(WebSocket webSocket)
    {
        ArgumentNullException.ThrowIfNull(webSocket);
        return new WebSocketRpcConnection(webSocket, proxyTypes);
    }

    public WebSocketRpcConnectionBuilder WithProxy<TProxy>()
        where TProxy : class
    {
        proxyTypes.Add(typeof(TProxy));
        return this;
    }
}
