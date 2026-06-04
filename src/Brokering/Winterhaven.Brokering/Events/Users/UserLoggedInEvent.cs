namespace Winterhaven.Brokering.Events.Users;

/// <summary>
/// </summary>
/// <param name="Username">
/// </param>
/// <param name="AccessToken">
/// </param>
public sealed record UserLoggedInEvent(
    string Username,
    string AccessToken);
