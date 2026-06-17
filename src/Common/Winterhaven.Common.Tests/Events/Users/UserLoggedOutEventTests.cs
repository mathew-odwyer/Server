using System;
using NUnit.Framework;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Users;

namespace Winterhaven.Common.Tests.Events.Users;

[TestFixture]
internal sealed class UserLoggedOutEventTests
{
    private string accessToken;

    private UserLoggedOutEvent notification;

    private Guid userAccountId;

    [Test]
    public void AccessTokenShouldReturnSameTakeWhenInvoked()
    {
        // Act
        string actual = this.notification.AccessToken;

        // Assert
        Assert.That(actual, Is.EqualTo(this.accessToken));
    }

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenAccessTokenIsEmpty()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new UserLoggedOutEvent(this.userAccountId, string.Empty));
    }

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenAccessTokenIsWhiteSpace()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new UserLoggedOutEvent(this.userAccountId, "\r\n\t"));
    }

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenUserAccountIdIsEmpty()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new UserLoggedOutEvent(Guid.Empty, "accessToken"));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenAccessTokenIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserLoggedOutEvent(this.userAccountId, null));
    }

    [Test]
    public void GetPublishEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        const string expected = "user.logged_out";
        var options = new PublishOptions();

        // Act
        string actual = UserLoggedOutEvent.GetPublishEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetSubscribeEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        const string expected = "user.logged_out";
        var options = new SubscribeOptions();

        // Act
        string actual = UserLoggedOutEvent.GetSubscribeEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [SetUp]
    public void SetUp()
    {
        this.userAccountId = Guid.NewGuid();
        this.accessToken = "accessToken";

        this.notification = new UserLoggedOutEvent(
            userAccountId: this.userAccountId,
            accessToken: this.accessToken);
    }

    [Test]
    public void UserAccountIdShouldReturnSameUserAccountIdWhenInvoked()
    {
        // Act
        var actual = this.notification.UserAccountId;

        // Assert
        Assert.That(actual, Is.EqualTo(this.userAccountId));
    }
}
