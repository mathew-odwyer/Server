namespace Winterhaven.Brokering.Events.Users;

/// <summary>
///   Represents an event that is triggered when a user has logged out.
/// </summary>
/// <param name="Username">
///   The username of the user who has logged out.
/// </param>
public sealed record UserLoggedOutEvent(
    string Username);