using System;
using NUnit.Framework;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Users;

namespace Winterhaven.Common.Tests.Events.Users;

[TestFixture]
internal sealed class UserLoggedInEventTests
{
    private string accessToken;

    private UserLoggedInEvent notification;

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
        Assert.Throws<ArgumentException>(() => new UserLoggedInEvent(this.userAccountId, string.Empty));
    }

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenAccessTokenIsWhiteSpace()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new UserLoggedInEvent(this.userAccountId, "\r\n\t"));
    }

    [Test]
    public void ConstructorShouldThrowArgumentExceptionWhenUserAccountIdIsEmpty()
    {
        // Act and assert
        Assert.Throws<ArgumentException>(() => new UserLoggedInEvent(Guid.Empty, "accessToken"));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenAccessTokenIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserLoggedInEvent(this.userAccountId, null));
    }

    [Test]
    public void GetPublishEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        const string expected = "user.logged_in";
        var options = new PublishOptions();

        // Act
        string actual = UserLoggedInEvent.GetPublishEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetSubscribeEventRouteShouldReturnCorrectRouteWhenInvoked()
    {
        // Arrange
        const string expected = "user.logged_in";
        var options = new SubscribeOptions();

        // Act
        string actual = UserLoggedInEvent.GetSubscribeEventRoute(options);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [SetUp]
    public void SetUp()
    {
        this.userAccountId = Guid.NewGuid();
        this.accessToken = "accessToken";

        this.notification = new UserLoggedInEvent(
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
