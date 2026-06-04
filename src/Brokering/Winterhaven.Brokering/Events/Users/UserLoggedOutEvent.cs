namespace Winterhaven.Brokering.Events.Users;

/// <summary>
/// </summary>
/// <param name="Username">
/// </param>
public sealed record UserLoggedOutEvent(
    string Username);
