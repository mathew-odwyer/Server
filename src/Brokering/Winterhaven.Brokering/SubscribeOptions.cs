using System;
using System.Collections.Generic;

namespace Winterhaven.Brokering;

/// <summary>
/// </summary>
public sealed class SubscribeOptions
{
    private readonly Dictionary<string, string> routeKeys;

    /// <summary>
    /// </summary>
    public SubscribeOptions() => routeKeys = [];

    /// <summary>
    /// </summary>
    public IReadOnlyDictionary<string, string> RouteKeys => routeKeys;

    /// <summary>
    /// </summary>
    /// <param name="key">
    /// </param>
    /// <param name="value">
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    public SubscribeOptions WithRouteKey(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (routeKeys.ContainsKey(key))
        {
            throw new ArgumentException($"The route key '{key}' has already been provided.", nameof(key));
        }

        routeKeys.Add(key, value);
        return this;
    }
}
