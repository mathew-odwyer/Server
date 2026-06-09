using System;

namespace Winterhaven.Brokering.Events.Players;

/// <summary>
/// </summary>
public sealed record PlayerNotifiedEvent : IEvent
{
    private const string RouteKey = "playerId";

    /// <summary>
    /// </summary>
    public required string Method { get; init; }

    /// <summary>
    /// </summary>
    public object? Params { get; init; }

    /// <inheritdoc/>
    public static string GetPublishEventRoute(PublishOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return $"player.{options.RouteKeys[RouteKey]}.notify";
    }

    /// <inheritdoc/>
    public static string GetSubscribeEventRoute(SubscribeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return $"player.{options.RouteKeys[RouteKey]}.notify";
    }
}
