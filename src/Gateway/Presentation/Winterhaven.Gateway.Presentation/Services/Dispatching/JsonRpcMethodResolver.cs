namespace Winterhaven.Gateway.Presentation.Services.Dispatching;

using System;
using System.Reflection;
using Winterhaven.Brokering.Attributes;
using Winterhaven.Gateway.Presentation.Exceptions;

internal sealed class JsonRpcMethodResolver : IJsonRpcMethodResolver
{
    public string ResolveMethodName<TNotification>(TNotification notification)
        where TNotification : class
    {
        ArgumentNullException.ThrowIfNull(notification);

        var type = typeof(TNotification);
        var attribute = type.GetCustomAttribute<NotificationNameAttribute>()
            ?? throw new UnmappedNotificationException(type);

        return attribute.Name;
    }
}