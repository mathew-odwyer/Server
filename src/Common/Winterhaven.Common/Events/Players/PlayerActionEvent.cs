using System;
using System.Collections.Generic;
using Winterhaven.Common.DTOs.Players;

namespace Winterhaven.Common.Events.Players;

/// <summary>
///   Represnts an event that is published when a player should perform an action.
/// </summary>
public sealed record PlayerActionEvent : IEvent
{
    private const string RouteKey = "playerId";

    /// <summary>
    ///   Initializes a new instance of the <see cref="PlayerActionEvent"/> class.
    /// </summary>
    /// <param name="actionQueue">
    ///   The action queue to be acted upon.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown if <paramref name="actionQueue"/> is <c>null</c>.
    /// </exception>
    public PlayerActionEvent(PlayerActionDto[] actionQueue)
    {
        this.ActionQueue = actionQueue ?? throw new ArgumentNullException(nameof(actionQueue));
    }

    /// <summary>
    ///   Gets the action queue to be acted upon.
    /// </summary>
    /// <value>
    ///   The action queue to be acted upon.
    /// </value>
    public IEnumerable<PlayerActionDto> ActionQueue { get; }

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
