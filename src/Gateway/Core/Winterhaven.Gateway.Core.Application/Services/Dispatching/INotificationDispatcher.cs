namespace Winterhaven.Gateway.Core.Application.Services.Dispatching;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INotificationDispatcher
{
    Task BroadcastAsync<TNotification>(TNotification notification)
        where TNotification : class;

    Task BroadcastAsync<TNotification>(IEnumerable<Guid> connectionIds, TNotification notification)
        where TNotification : class;

    Task NotifyAsync<TNotification>(Guid connectionId, TNotification notification)
                where TNotification : class;
}