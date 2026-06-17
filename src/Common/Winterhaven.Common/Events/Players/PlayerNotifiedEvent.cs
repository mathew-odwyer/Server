using System;

namespace Winterhaven.Common.Events.Players;

/// <summary>
///   Represents an event that is published when the player should be notified from another service.
/// </summary>
public sealed record PlayerNotifiedEvent : IEvent
{
    private const string RouteKey = "playerId";

    /// <summary>
    ///   Initializes a new instance of the <see cref="PlayerNotifiedEvent"/> class.
    /// </summary>
    /// <param name="method">
    ///   The method to be invoked on the client.
    /// </param>
    /// <param name="parameters">
    ///   The parameters of the method to be invoked on the client.
    /// </param>
    public PlayerNotifiedEvent(string method, object? parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(method);

        this.Method = method;
        this.Parameters = parameters;
    }

    /// <summary>
    ///   Gets the method to be invoked on the client.
    /// </summary>
    /// <value>
    ///   The method to be invoked on the client.
    /// </value>
    public string Method { get; }

    /// <summary>
    ///   Gets the parameters of the method to be invoked on the client.
    /// </summary>
    /// <value>
    ///   The parameters of the method to be invoked on the client.
    /// </value>
    public object? Parameters { get; }

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
