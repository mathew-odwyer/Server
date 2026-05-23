namespace Winterhaven.Brokering.Events.Players;

using Winterhaven.Brokering.Attributes;

[EventName("player.{UserAccountId}.notify")]
public sealed record PlayerNotificationEvent(
    string Method,
    object? Params);