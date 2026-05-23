namespace Winterhaven.Gateway.Presentation.Services.Dispatching;

using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Dispatching;

internal sealed class JsonRpcNotificationDispatcher : INotificationDispatcher
{
    private readonly ConcurrentDictionary<Guid, JsonRpc> connectionIdToJsonRpcMap = new();

    private readonly ILogger<JsonRpcNotificationDispatcher> logger;

    private readonly IJsonRpcMethodResolver methodResolver;

    public JsonRpcNotificationDispatcher(ILogger<JsonRpcNotificationDispatcher> logger, IJsonRpcMethodResolver methodResolver)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.methodResolver = methodResolver ?? throw new ArgumentNullException(nameof(methodResolver));
    }

    public async Task BroadcastAsync<TNotification>(TNotification notification)
        where TNotification : class
    {
        ArgumentNullException.ThrowIfNull(notification);
        await this.BroadcastAsync(this.connectionIdToJsonRpcMap.Keys, notification).ConfigureAwait(false);
    }

    public async Task BroadcastAsync<TNotification>(IEnumerable<Guid> connectionIds, TNotification notification)
        where TNotification : class
    {
        ArgumentNullException.ThrowIfNull(notification);
        ArgumentNullException.ThrowIfNull(connectionIds);

        string method = this.methodResolver.ResolveMethodName(notification);

        var tasks = connectionIds
            .Where(this.connectionIdToJsonRpcMap.ContainsKey)
            .Select(x => this.connectionIdToJsonRpcMap[x].NotifyAsync(method, notification));

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public void Deregister(Guid connectionId)
    {
        this.logger.LogTrace("Deregistering connection: '{ConnectionId}'", connectionId);

        if (!this.connectionIdToJsonRpcMap.TryRemove(connectionId, out _))
        {
            this.logger.LogWarning("Connection with ID: '{ConnectionId}' has already been deregistered.", connectionId);
        }
    }

    public async Task NotifyAsync<TNotification>(Guid connectionId, TNotification notification)
        where TNotification : class
    {
        ArgumentNullException.ThrowIfNull(notification);

        string method = this.methodResolver.ResolveMethodName(notification);

        if (this.connectionIdToJsonRpcMap.TryGetValue(connectionId, out var rpc))
        {
            await rpc.NotifyAsync(method, notification).ConfigureAwait(false);
        }
    }

    public void Register(Guid connectionId, JsonRpc rpc)
    {
        ArgumentNullException.ThrowIfNull(rpc);

        this.logger.LogTrace("Registering connection: '{ConnectionId}'", connectionId);

        if (!this.connectionIdToJsonRpcMap.TryAdd(connectionId, rpc))
        {
            this.logger.LogWarning("Connection with ID: '{ConnectionId}' has already been registered.", connectionId);
        }
    }
}