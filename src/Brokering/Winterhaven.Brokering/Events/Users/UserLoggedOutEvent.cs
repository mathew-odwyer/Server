using System;

namespace Winterhaven.Brokering.Events.Users;

/// <summary>
/// </summary>
public sealed record UserLoggedOutEvent : IEvent
{
    private const string EventRoute = "user.logged_out";

    /// <summary>
    /// </summary>
    public required Guid UserAccountId { get; init; }

    /// <summary>
    /// </summary>
    public required string AccessToken { get; init; }

    /// <inheritdoc/>
    public static string GetPublishEventRoute(PublishOptions options) => EventRoute;

    /// <inheritdoc/>
    public static string GetSubscribeEventRoute(SubscribeOptions options) => EventRoute;
}
