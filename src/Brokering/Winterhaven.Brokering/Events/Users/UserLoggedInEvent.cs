namespace Winterhaven.Brokering.Events.Users;

/// <summary>
///   Represents an event that is triggered when a user successfully logs in.
/// </summary>
/// <param name="Username">
///   The username of the user who has logged in.
/// </param>
/// <param name="AccessToken">
///   The access token issued to the user upon successful login, which can be used for authentication of subsequent requests.
/// </param>
public sealed record UserLoggedInEvent(
    string Username,
    string AccessToken);
