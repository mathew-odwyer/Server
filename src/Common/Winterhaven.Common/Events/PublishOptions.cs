using System;
using System.Collections.Generic;

namespace Winterhaven.Common.Events;

/// <summary>
/// </summary>
public sealed class PublishOptions
{
    private readonly Dictionary<string, string> routeKeys;

    /// <summary>
    /// </summary>
    public PublishOptions() => routeKeys = [];

    /// <summary>
    /// </summary>
    public IReadOnlyDictionary<string, string> RouteKeys => routeKeys;

    /// <summary>
    /// </summary>
    /// <exception cref="ArgumentException">
    /// </exception>
    public PublishOptions WithRouteKey(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (routeKeys.ContainsKey(key))
            throw new ArgumentException($"The route key '{key}' has already been provided.", nameof(key));

        routeKeys.Add(key, value);
        return this;
    }
}
