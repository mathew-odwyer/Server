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
    public PublishOptions()
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
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentException">
    /// </exception>
    public PublishOptions WithRouteKey(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (this.routeKeys.ContainsKey(key))
            throw new ArgumentException($"The route key '{key}' has already been provided.", nameof(key));

        this.routeKeys.Add(key, value);
        return this;
    }
}
