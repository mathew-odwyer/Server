namespace Winterhaven.Gateway.Core.Application.Notifications.Players;

using Winterhaven.Brokering.Attributes;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Players;

[NotificationName("player.joined")]
internal sealed record PlayerJoinedNotification(Player Player);