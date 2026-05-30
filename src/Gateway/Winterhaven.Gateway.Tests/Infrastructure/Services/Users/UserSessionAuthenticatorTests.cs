using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Tests.Infrastructure.Services.Users;

[TestFixture]
internal sealed class UserSessionAuthenticatorTests
{
    private UserSessionAuthenticator authenticator;

    private ILogger<UserSessionAuthenticator> logger;

    [Test]
    public void AuthenticateShouldMarkAsAuthenticated()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", TimeSpan.FromHours(1));

        // Act
        authenticator.Authenticate(session);

        // Assert
        Assert.That(authenticator.IsAuthenticated, Is.True);
    }

    [Test]
    public void AuthenticateShouldNotOverrideExistingSession()
    {
        // Arrange
        var session1 = new UserSession(Guid.NewGuid(), "User1", "token1", TimeSpan.FromHours(1));
        var session2 = new UserSession(Guid.NewGuid(), "User2", "token2", TimeSpan.FromHours(2));
        authenticator.Authenticate(session1);

        // Act
        authenticator.Authenticate(session2);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(session1));
    }

    [Test]
    public void AuthenticateShouldSetUserSession()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", TimeSpan.FromHours(1));

        // Act
        authenticator.Authenticate(session);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(session));
    }

    [Test]
    public void AuthenticateShouldThrowArgumentNullExceptionWhenSessionIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => authenticator.Authenticate(null));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserSessionAuthenticator(null));

    [Test]
    public void InvalidateShouldClearUserSession()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", TimeSpan.FromHours(1));
        authenticator.Authenticate(session);

        // Act
        authenticator.Invalidate();

        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void InvalidateShouldDoNothingWhenNotAuthenticated()
    {
        // Act
        authenticator.Invalidate();

        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void InvalidateShouldMarkAsNotAuthenticated()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", TimeSpan.FromHours(1));
        authenticator.Authenticate(session);

        // Act
        authenticator.Invalidate();

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
        var session = new UserSession(Guid.NewGuid(), "User1", "token", TimeSpan.FromHours(1));

        // Act
        authenticator.Refresh(session);

        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void RefreshShouldThrowArgumentNullExceptionWhenSessionIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => authenticator.Refresh(null));

    [Test]
    public void RefreshShouldUpdateUserSessionWhenAuthenticated()
    {
        // Arrange
        var session1 = new UserSession(Guid.NewGuid(), "User1", "token1", TimeSpan.FromHours(1));
        var session2 = new UserSession(Guid.NewGuid(), "User1", "token2", TimeSpan.FromHours(2));
        authenticator.Authenticate(session1);

        // Act
        authenticator.Refresh(session2);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(session2));
    }

    [SetUp]
    public void Setup()
    {
        // Arrange
        logger = Substitute.For<ILogger<UserSessionAuthenticator>>();
        authenticator = new UserSessionAuthenticator(logger);
    }

    [Test]
    public void UserSessionShouldBeNullByDefault() =>
        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
}
