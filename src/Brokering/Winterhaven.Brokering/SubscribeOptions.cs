using System;
using System.Collections.Generic;

namespace Winterhaven.Brokering;

/// <summary>
/// </summary>
public sealed class SubscribeOptions
{
    private readonly Dictionary<string, string> routeKeys = [];

    /// <summary>
    /// </summary>
    public IReadOnlyDictionary<string, string> RouteKeys => routeKeys;

    /// <summary>
    /// </summary>
    public SubscribeOptions WithRouteKey(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        routeKeys[key] = value;

        return this;
    }
}
