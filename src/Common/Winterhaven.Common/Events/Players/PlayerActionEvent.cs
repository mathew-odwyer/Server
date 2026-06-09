using System;
using Winterhaven.Common.DTOs.Players;

namespace Winterhaven.Common.Events.Players;

/// <summary>
/// </summary>
public sealed record PlayerActionEvent : IEvent
{
    private const string RouteKey = "playerId";

    /// <summary>
    /// </summary>
    /// <param name="actionQueue"></param>
    public PlayerActionEvent(PlayerActionDto[] actionQueue)
    {
        this.ActionQueue = actionQueue ?? throw new ArgumentNullException(nameof(actionQueue));
    }

    /// <summary>
    /// </summary>
    public PlayerActionDto[] ActionQueue { get; }

    /// <inheritdoc/>
    public static string GetPublishEventRoute(PublishOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return $"player.{options.RouteKeys[RouteKey]}.action";
    }

    /// <inheritdoc/>
    public static string GetSubscribeEventRoute(SubscribeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return $"player.{options.RouteKeys[RouteKey]}.action";
    }
}
