// <copyright file="UserSessionTokenTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Domain.Entities.Users;

using NUnit.Framework;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class UserSessionTokenTests
{
    private readonly Guid sessionId = Guid.NewGuid();

    private UserSessionToken userSessionToken;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new UserSessionToken()
        {
            UserAccountId = "0",
            SessionId = this.sessionId,
            HashedRefreshToken = "Test",
        });
    }

    [Test]
    public void ExpirationDateShouldEqualDefaultWhenInvoked()
    {
        // Arrange
        var expected = default(DateTime);

        // Act
        var actual = this.userSessionToken.ExpirationDate;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ExpirationDateShouldEqualUtcNowWhenInvoked()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        this.userSessionToken.ExpirationDate = expected;

        // Act
        var actual = this.userSessionToken.ExpirationDate;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void HashedRefreshTokenShouldEqualTestWhenInvoked()
    {
        // Arrange
        const string expected = "Test";

        // Act
        string actual = this.userSessionToken.HashedRefreshToken;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void SessionIdShouldEqualSessionIdWhenInvoked()
    {
        // Arrange
        var expected = this.sessionId;

        // Act
        var actual = this.userSessionToken.SessionId;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [SetUp]
    public void Setup()
    {
        // Arrange
        string userAccountId = "0";
        string hashedRefreshToken = "Test";
        var sessionId = this.sessionId;

        this.userSessionToken = new UserSessionToken()
        {
            UserAccountId = userAccountId,
            HashedRefreshToken = hashedRefreshToken,
            SessionId = sessionId,
        };
    }

    [Test]
    public void UserAccountIdShouldEqualZeroWhenInvoked()
    {
        // Arrange
        const string expected = "0";

        // Act
        string actual = this.userSessionToken.UserAccountId;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}
