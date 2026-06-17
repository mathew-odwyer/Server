using System;
using NUnit.Framework;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Players;

namespace Winterhaven.Common.Tests.Events.Players;

[TestFixture]
internal sealed class PlayerNotifiedEventTests
{
    private string method;

    private PlayerNotifiedEvent notification;

    private object parameters;

    private Guid playerId;

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenMethodIsEmpty()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new PlayerNotifiedEvent(string.Empty, this.parameters));
    }

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenMethodIsWhiteSpace()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new PlayerNotifiedEvent("\r\n\t", this.parameters));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMethodIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new PlayerNotifiedEvent(null, this.parameters));
    }

    [Test]
    public void GetPublishEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        string expected = $"player.{this.playerId}.notify";
        var options = new PublishOptions()
            .WithRouteKey("playerId", this.playerId.ToString());

        // Act
        string actual = PlayerNotifiedEvent.GetPublishEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetPublishEventRouteShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => PlayerNotifiedEvent.GetPublishEventRoute(null));
    }

    [Test]
    public void GetSubscribeEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        string expected = $"player.{this.playerId}.notify";
        var options = new SubscribeOptions()
            .WithRouteKey("playerId", this.playerId.ToString());

        // Act
        string actual = PlayerNotifiedEvent.GetSubscribeEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetSubscribeEventRouteShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => PlayerNotifiedEvent.GetSubscribeEventRoute(null));
    }

    [Test]
    public void MethodShouldReturnSameMethodWhenInvoked()
    {
        // Act
        string actual = this.notification.Method;

        // Assert
        Assert.That(actual, Is.EqualTo(this.method));
    }

    [Test]
    public void ParametersShouldReturnSameParametersWhenInvoked()
    {
        // Act
        object actual = this.notification.Parameters;

        // Assert
        Assert.That(actual, Is.EqualTo(this.parameters));
    }

    [SetUp]
    public void SetUp()
    {
        this.playerId = Guid.NewGuid();
        this.method = "method";
        this.parameters = new { Foo = "bar" };

        this.notification = new PlayerNotifiedEvent(
            method: this.method,
            parameters: this.parameters);
    }
}
