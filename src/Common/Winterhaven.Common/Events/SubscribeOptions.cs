using System;
using System.Collections.Generic;

namespace Winterhaven.Common.Events;

/// <summary>
/// </summary>
public sealed class SubscribeOptions
{
    private readonly Dictionary<string, string> routeKeys;

    /// <summary>
    /// </summary>
    public SubscribeOptions()
    {
        this.routeKeys = [];
    }

    /// <summary>
    /// </summary>
    public IReadOnlyDictionary<string, string> RouteKeys
    {
        get
        {
            return this.routeKeys;
        }
    }

    /// <summary>
    /// </summary>
    /// <exception cref="ArgumentException">
    /// </exception>
    public SubscribeOptions WithRouteKey(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (this.routeKeys.ContainsKey(key))
            throw new ArgumentException($"The route key '{key}' has already been provided.", nameof(key));

        this.routeKeys.Add(key, value);
        return this;
    }
}
