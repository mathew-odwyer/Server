namespace Winterhaven.Brokering.Events.Users;

using Winterhaven.Brokering.Attributes;

/// <summary>
///   Represents an event that is triggered when a user has logged in.
/// </summary>
/// <param name="Username">
///   The username of the user who has logged in.
/// </param>
/// <param name="AccessToken">
///   The access token of the user who has logged in.
/// </param>
[EventName("user.logged_in")]
public sealed record UserLoggedInEvent(
    string Username,
    string AccessToken);