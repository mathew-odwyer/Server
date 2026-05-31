using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Tests.Infrastructure.Services.Users;

[TestFixture]
internal sealed class UserSessionManagerTests
{
    private UserSessionManager authenticator;

    private ILogger<UserSessionManager> logger;

    private FakeTimeProvider timeProvider;

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
        Assert.Throws<ArgumentNullException>(() => new UserSessionManager(null, timeProvider));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenTimeProviderIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserSessionManager(logger, null));

    [Test]
    public void DisposeCalledMultipleTimesShouldNotThrow() =>
        // Act and assert
        Assert.DoesNotThrow(() =>
        {
            authenticator.Dispose();
            authenticator.Dispose();
        });

    [Test]
    public void EstablishUserSessionShouldThrowObjectDisposedExceptionWhenDisposed()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.Dispose();

        // Act and assert
        Assert.Throws<ObjectDisposedException>(() => authenticator.EstablishUserSession(session));
    }

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
    public void InvalidateUserSessionShouldThrowObjectDisposedExceptionWhenDisposed()
    {
        // Arrange
        authenticator.Dispose();

        // Act and assert
        Assert.Throws<ObjectDisposedException>(authenticator.InvalidateUserSession);
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
    public async Task RefreshShouldDoNothingWhenSessionTokenHasAlreadyExpired()
    {
        // Arrange

        var expiredSession = new UserSession(Guid.NewGuid(), "User1", "token1", DateTimeOffset.UtcNow.AddMinutes(-1));
        var newSession = new UserSession(Guid.NewGuid(), "User1", "token2", DateTimeOffset.UtcNow.AddMinutes(15));

        authenticator.EstablishUserSession(expiredSession);

        //// Give the zero-delay CancelAfter time to fire before attempting the refresh.
        await Task.Delay(50).ConfigureAwait(false);

        // Act
        authenticator.RefreshUserSession(newSession);

        // Assert
        Assert.That(authenticator.UserSession, Is.EqualTo(expiredSession));
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

    [Test]
    public void RefreshUserSessionShouldThrowObjectDisposedExceptionWhenDisposed()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "User1", "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.Dispose();

        // Act and assert
        Assert.Throws<ObjectDisposedException>(() => authenticator.RefreshUserSession(session));
    }

    [SetUp]
    public void Setup()
    {
        // Arrange
        logger = Substitute.For<ILogger<UserSessionManager>>();

        timeProvider = new FakeTimeProvider();
        timeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        authenticator = new UserSessionManager(logger, timeProvider);
    }

    [TearDown]
    public void TearDown() => authenticator.Dispose();

    [Test]
    public void UserSessionShouldBeNullByDefault() =>
        // Assert
        Assert.That(authenticator.UserSession, Is.Null);
}
