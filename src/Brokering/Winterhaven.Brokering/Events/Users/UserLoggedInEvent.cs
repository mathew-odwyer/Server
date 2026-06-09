using System;

namespace Winterhaven.Brokering.Events.Users;

/// <summary>
/// </summary>
public sealed record UserLoggedInEvent : IEvent
{
    private const string EventRoute = "user.logged_in";

    /// <summary>
    /// </summary>
    /// <param name="userAccountId">
    /// </param>
    /// <param name="accessToken">
    /// </param>
    public UserLoggedInEvent(Guid userAccountId, string accessToken)
    {
        if (userAccountId == Guid.Empty)
        {
            throw new ArgumentException($"{userAccountId} must not be empty.", nameof(userAccountId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        UserAccountId = userAccountId;
        AccessToken = accessToken;
    }

    /// <summary>
    /// </summary>
    public string AccessToken { get; init; }

    /// <summary>
    /// </summary>
    public Guid UserAccountId { get; init; }

    /// <inheritdoc/>
    public static string GetPublishEventRoute(PublishOptions options) => EventRoute;

    /// <inheritdoc/>
    public static string GetSubscribeEventRoute(SubscribeOptions options) => EventRoute;
}
