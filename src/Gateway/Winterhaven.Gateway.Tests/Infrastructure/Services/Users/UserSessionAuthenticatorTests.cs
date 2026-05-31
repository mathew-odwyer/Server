using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Tests.Infrastructure.Services.Users;

[TestFixture]
internal sealed class userSessionManagerTests
{
    private UserSessionManager authenticator;

    private ILogger<UserSessionManager> logger;

    [Test]
    public void AuthenticateShouldMarkAsAuthenticated()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));

        // Act
        authenticator.EstablishUserSession(session);

        // Assert
        Assert.That(authenticator.IsAuthenticated, Is.True);
    }

    [Test]
    public void AuthenticateShouldNotOverrideExistingSession()
    {
        // Arrange
        var session1 = new UserSession(Guid.NewGuid(), "User1", "token1", DateTimeOffset.UtcNow.AddMinutes(15));
        var session2 = new UserSession(Guid.NewGuid(), "User2", "token2", DateTimeOffset.UtcNow.AddMinutes(16));
        authenticator.EstablishUserSession(session1);

        // Act
        authenticator.EstablishUserSession(session2);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(session1));
    }

    [Test]
    public void AuthenticateShouldSetUserSession()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));

        // Act
        authenticator.EstablishUserSession(session);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(session));
    }

    [Test]
    public void AuthenticateShouldThrowArgumentNullExceptionWhenSessionIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => authenticator.EstablishUserSession(null));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserSessionManager(null));

    [Test]
    public void InvalidateShouldClearUserSession()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.EstablishUserSession(session);

        // Act
        authenticator.InvalidateUserSession();

        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void InvalidateShouldDoNothingWhenNotAuthenticated()
    {
        // Act
        authenticator.InvalidateUserSession();

        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void InvalidateShouldMarkAsNotAuthenticated()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.EstablishUserSession(session);

        // Act
        authenticator.InvalidateUserSession();

        // Assert
        Assert.That(authenticator.IsAuthenticated, Is.False);
    }

    [Test]
    public void IsAuthenticatedShouldBeFalseByDefault() =>
        // Assert
        Assert.That(authenticator.IsAuthenticated, Is.False);

    [Test]
    public void RefreshShouldDoNothingWhenNotAuthenticated()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));

        // Act
        authenticator.RefreshUserSession(session);

        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void RefreshShouldThrowArgumentNullExceptionWhenSessionIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => authenticator.RefreshUserSession(null));

    [Test]
    public void RefreshShouldUpdateUserSessionWhenAuthenticated()
    {
        // Arrange
        var session1 = new UserSession(Guid.NewGuid(), "User1", "token1", DateTimeOffset.UtcNow.AddMinutes(15));
        var session2 = new UserSession(Guid.NewGuid(), "User1", "token2", DateTimeOffset.UtcNow.AddMinutes(16));
        authenticator.EstablishUserSession(session1);

        // Act
        authenticator.RefreshUserSession(session2);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(session2));
    }

    [SetUp]
    public void Setup()
    {
        // Arrange
        logger = Substitute.For<ILogger<UserSessionManager>>();
        authenticator = new UserSessionManager(logger);
    }

    [TearDown]
    public void TearDown() => authenticator.Dispose();

    [Test]
    public void UserSessionShouldBeNullByDefault() =>
        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
}
