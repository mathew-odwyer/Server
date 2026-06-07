using System;

namespace Winterhaven.Brokering.Events.Players;

/// <summary>
/// </summary>
/// <param name="Type">
/// </param>
/// <param name="MoveX">
/// </param>
/// <param name="MoveY">
/// </param>
/// <param name="Identifier">
/// </param>
public sealed record PlayerActionEventData(
    string Type,
    double MoveX,
    double MoveY,
    double Identifier);

/// <summary>
/// </summary>
public sealed record PlayerActionEvent : IEvent
{
    private const string RouteKey = "playerId";

    /// <summary>
    /// </summary>
    /// <param name="actionQueue">
    /// </param>
    public PlayerActionEvent(PlayerActionEventData[] actionQueue) =>
        ActionQueue = actionQueue ?? throw new ArgumentNullException(nameof(actionQueue));

    /// <summary>
    /// </summary>
    public PlayerActionEventData[] ActionQueue { get; }

    /// <summary>
    /// </summary>
    /// <param name="options">
    /// </param>
    /// <returns>
    /// </returns>
    public static string GetPublishEventRoute(PublishOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return $"player.{options.RouteKeys[RouteKey]}.action";
    }

    /// <summary>
    /// </summary>
    /// <param name="options">
    /// </param>
    /// <returns>
    /// </returns>
    public static string GetSubscribeEventRoute(SubscribeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return $"player.{options.RouteKeys[RouteKey]}.action";
    }
}
