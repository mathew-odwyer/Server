using System;

namespace Winterhaven.Common.Events.Users;

/// <summary>
///   Represents an event that is published when a user has logged out.
/// </summary>
public sealed record UserLoggedOutEvent : IEvent
{
    private const string EventRoute = "user.logged_out";

    /// <summary>
    ///   Initializes a new instance of the <see cref="UserLoggedOutEvent"/> class.
    /// </summary>
    /// <param name="userAccountId">
    ///   The user account identifier of the user who has logged out.
    /// </param>
    /// <param name="accessToken">
    ///   The access token of the user who has logged out.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown if <paramref name="accessToken"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   Thrown if <paramref name="accessToken"/> is empty or consists of white space or <paramref name="userAccountId"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    public UserLoggedOutEvent(Guid userAccountId, string accessToken)
    {
        if (userAccountId == Guid.Empty)
            throw new ArgumentException($"{userAccountId} must not be empty.", nameof(userAccountId));

        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        this.UserAccountId = userAccountId;
        this.AccessToken = accessToken;
    }

    /// <summary>
    ///   Gets the access token of the user who has logged out.
    /// </summary>
    /// <value>
    ///   The access token of the user who has logged out.
    /// </value>
    public string AccessToken { get; init; }

    /// <summary>
    ///   Gets the user account identifier of the user who has logged out.
    /// </summary>
    /// <value>
    ///   The user account identifier of the user who has logged out.
    /// </value>
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
