namespace Winterhaven.Brokering.Events.Players;

public sealed record PlayerNotificationEvent(
    string Method,
    object? Params);