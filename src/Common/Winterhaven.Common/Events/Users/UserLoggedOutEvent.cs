using System;

namespace Winterhaven.Common.Events.Users;

/// <summary>
/// </summary>
public sealed record UserLoggedOutEvent : IEvent
{
    private const string EventRoute = "user.logged_out";

    /// <summary>
    /// </summary>
    /// <param name="userAccountId"></param>
    /// <param name="accessToken"></param>
    public UserLoggedOutEvent(Guid userAccountId, string accessToken)
    {
        if (userAccountId == Guid.Empty)
            throw new ArgumentException($"{userAccountId} must not be empty.", nameof(userAccountId));

        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        this.UserAccountId = userAccountId;
        this.AccessToken = accessToken;
    }

    /// <summary>
    /// </summary>
    public string AccessToken { get; init; }

    /// <summary>
    /// </summary>
    public Guid UserAccountId { get; init; }

    /// <inheritdoc/>
    public static string GetPublishEventRoute(PublishOptions options)
    {
        return EventRoute;
    }

    /// <inheritdoc/>
    public static string GetSubscribeEventRoute(SubscribeOptions options)
    {
        return EventRoute;
    }
}
