using System;
using NUnit.Framework;
using Winterhaven.Common.DTOs.Players;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Players;

namespace Winterhaven.Common.Tests.Events.Players;

[TestFixture]
internal sealed class PlayerActionEventTests
{
    private PlayerActionDto[] actionQueue;

    private PlayerActionEvent notification;

    private Guid playerId;

    [Test]
    public void ActionQueueShouldReturnSameActionQueueWhenInvoked()
    {
        // Act
        var actual = this.notification.ActionQueue;

        // Assert
        Assert.That(actual, Is.EqualTo(this.actionQueue));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenActionQueueIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new PlayerActionEvent(null));
    }

    [Test]
    public void GetPublishEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        string expected = $"player.{this.playerId}.action";
        var options = new PublishOptions()
            .WithRouteKey("playerId", this.playerId.ToString());

        // Act
        string actual = PlayerActionEvent.GetPublishEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetPublishEventRouteShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => PlayerActionEvent.GetPublishEventRoute(null));
    }

    [Test]
    public void GetSubscribeEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        string expected = $"player.{this.playerId}.action";
        var options = new SubscribeOptions()
            .WithRouteKey("playerId", this.playerId.ToString());

        // Act
        string actual = PlayerActionEvent.GetSubscribeEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetSubscribeEventRouteShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => PlayerActionEvent.GetSubscribeEventRoute(null));
    }

    [SetUp]
    public void SetUp()
    {
        this.playerId = Guid.NewGuid();
        this.actionQueue =
        [
            new PlayerActionDto(
                Type: "move",
                MoveX: 1,
                MoveY: 1,
                Identifier: 0),
        ];

        this.notification = new PlayerActionEvent(this.actionQueue);
    }
}
