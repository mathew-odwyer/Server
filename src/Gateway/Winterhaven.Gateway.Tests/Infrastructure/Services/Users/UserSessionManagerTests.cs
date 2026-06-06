using System;
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
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new UserSessionManager(null, timeProvider));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenTimeProviderIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new UserSessionManager(logger, null));

    [Test]
    public void DisposeCalledMultipleTimesShouldNotThrow() =>
        Assert.DoesNotThrow(() =>
        {
            authenticator.Dispose();
            authenticator.Dispose();
        });

    [Test]
    public void DisposeShouldStopExpiryTimerSoSessionIsNotInvalidatedAfterwards()
    {
        var expiry = TimeSpan.FromMinutes(15);
        var session = new UserSession(Guid.NewGuid(), "token", timeProvider.GetUtcNow() + expiry);
        authenticator.EstablishUserSession(session);

        authenticator.Dispose();

        Assert.DoesNotThrow(() => timeProvider.Advance(expiry));
    }

    [Test]
    public void EstablishUserSessionShouldDoNothingWhenSessionIsAlreadyExpired()
    {
        var expiredSession = new UserSession(Guid.NewGuid(), "token", timeProvider.GetUtcNow().AddMinutes(-1));

        authenticator.EstablishUserSession(expiredSession);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(authenticator.IsAuthenticated, Is.False);
            Assert.That(authenticator.UserSession, Is.Null);
        }
    }

    [Test]
    public void EstablishUserSessionShouldFireEstablishedEvent()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        int raisedCount = 0;
        authenticator.Established += (_, _) => raisedCount++;

        authenticator.EstablishUserSession(session);

        Assert.That(raisedCount, Is.EqualTo(1));
    }

    [Test]
    public void EstablishUserSessionShouldMarkAsAuthenticated()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));

        authenticator.EstablishUserSession(session);

        Assert.That(authenticator.IsAuthenticated, Is.True);
    }

    [Test]
    public void EstablishUserSessionShouldNotFireEstablishedEventWhenAlreadyAuthenticated()
    {
        var session1 = new UserSession(Guid.NewGuid(), "token1", DateTimeOffset.UtcNow.AddMinutes(15));
        var session2 = new UserSession(Guid.NewGuid(), "token2", DateTimeOffset.UtcNow.AddMinutes(16));
        authenticator.EstablishUserSession(session1);

        int raisedCount = 0;
        authenticator.Established += (_, _) => raisedCount++;
        authenticator.EstablishUserSession(session2);

        Assert.That(raisedCount, Is.Zero);
    }

    [Test]
    public void EstablishUserSessionShouldNotFireEstablishedEventWhenSessionIsAlreadyExpired()
    {
        var expiredSession = new UserSession(Guid.NewGuid(), "token", timeProvider.GetUtcNow().AddMinutes(-1));
        int raisedCount = 0;
        authenticator.Established += (_, _) => raisedCount++;

        authenticator.EstablishUserSession(expiredSession);

        Assert.That(raisedCount, Is.Zero);
    }

    [Test]
    public void EstablishUserSessionShouldNotOverrideExistingSession()
    {
        var session1 = new UserSession(Guid.NewGuid(), "token1", DateTimeOffset.UtcNow.AddMinutes(15));
        var session2 = new UserSession(Guid.NewGuid(), "token2", DateTimeOffset.UtcNow.AddMinutes(16));
        authenticator.EstablishUserSession(session1);

        authenticator.EstablishUserSession(session2);

        Assert.That(authenticator.UserSession, Is.EqualTo(session1));
    }

    [Test]
    public void EstablishUserSessionShouldSetUserSession()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));

        authenticator.EstablishUserSession(session);

        Assert.That(authenticator.UserSession, Is.EqualTo(session));
    }

    [Test]
    public void EstablishUserSessionShouldThrowArgumentNullExceptionWhenSessionIsNull() =>
        Assert.Throws<ArgumentNullException>(() => authenticator.EstablishUserSession(null));

    [Test]
    public void EstablishUserSessionShouldThrowObjectDisposedExceptionWhenDisposed()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.Dispose();

        Assert.Throws<ObjectDisposedException>(() => authenticator.EstablishUserSession(session));
    }

    [Test]
    public void InvalidateUserSessionShouldClearUserSession()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.EstablishUserSession(session);

        authenticator.InvalidateUserSession();

        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void InvalidateUserSessionShouldDoNothingWhenNotAuthenticated()
    {
        authenticator.InvalidateUserSession();

        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void InvalidateUserSessionShouldFireInvalidatedEvent()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.EstablishUserSession(session);

        int raisedCount = 0;
        authenticator.Invalidated += (_, _) => raisedCount++;
        authenticator.InvalidateUserSession();

        Assert.That(raisedCount, Is.EqualTo(1));
    }

    [Test]
    public void InvalidateUserSessionShouldMarkAsNotAuthenticated()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.EstablishUserSession(session);

        authenticator.InvalidateUserSession();

        Assert.That(authenticator.IsAuthenticated, Is.False);
    }

    [Test]
    public void InvalidateUserSessionShouldNotFireInvalidatedEventWhenNotAuthenticated()
    {
        int raisedCount = 0;
        authenticator.Invalidated += (_, _) => raisedCount++;

        authenticator.InvalidateUserSession();

        Assert.That(raisedCount, Is.Zero);
    }

    [Test]
    public void InvalidateUserSessionShouldThrowObjectDisposedExceptionWhenDisposed()
    {
        authenticator.Dispose();

        Assert.Throws<ObjectDisposedException>(authenticator.InvalidateUserSession);
    }

    [Test]
    public void IsAuthenticatedShouldBeFalseByDefault() =>
        Assert.That(authenticator.IsAuthenticated, Is.False);

    [Test]
    public void RefreshUserSessionShouldDoNothingWhenNewSessionIsAlreadyExpired()
    {
        var validSession = new UserSession(Guid.NewGuid(), "token1", timeProvider.GetUtcNow().AddMinutes(15));
        var expiredSession = new UserSession(Guid.NewGuid(), "token2", timeProvider.GetUtcNow().AddMinutes(-1));
        authenticator.EstablishUserSession(validSession);

        authenticator.RefreshUserSession(expiredSession);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(authenticator.UserSession, Is.EqualTo(validSession));
            Assert.That(authenticator.IsAuthenticated, Is.True);
        }
    }

    [Test]
    public void RefreshUserSessionShouldDoNothingWhenNotAuthenticated()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));

        authenticator.RefreshUserSession(session);

        Assert.That(authenticator.UserSession, Is.Null);
    }

    [Test]
    public void RefreshUserSessionShouldFireRefreshedEvent()
    {
        var session1 = new UserSession(Guid.NewGuid(), "token1", DateTimeOffset.UtcNow.AddMinutes(15));
        var session2 = new UserSession(Guid.NewGuid(), "token2", DateTimeOffset.UtcNow.AddMinutes(16));
        authenticator.EstablishUserSession(session1);

        int raisedCount = 0;
        authenticator.Refreshed += (_, _) => raisedCount++;
        authenticator.RefreshUserSession(session2);

        Assert.That(raisedCount, Is.EqualTo(1));
    }

    [Test]
    public void RefreshUserSessionShouldNotFireRefreshedEventWhenNewSessionIsAlreadyExpired()
    {
        var validSession = new UserSession(Guid.NewGuid(), "token1", timeProvider.GetUtcNow().AddMinutes(15));
        var expiredSession = new UserSession(Guid.NewGuid(), "token2", timeProvider.GetUtcNow().AddMinutes(-1));
        authenticator.EstablishUserSession(validSession);

        int raisedCount = 0;
        authenticator.Refreshed += (_, _) => raisedCount++;
        authenticator.RefreshUserSession(expiredSession);

        Assert.That(raisedCount, Is.Zero);
    }

    [Test]
    public void RefreshUserSessionShouldNotFireRefreshedEventWhenNotAuthenticated()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        int raisedCount = 0;
        authenticator.Refreshed += (_, _) => raisedCount++;

        authenticator.RefreshUserSession(session);

        Assert.That(raisedCount, Is.Zero);
    }

    [Test]
    public void RefreshUserSessionShouldResetExpiryTimer()
    {
        var now = timeProvider.GetUtcNow();
        var session1 = new UserSession(Guid.NewGuid(), "token1", now + TimeSpan.FromMinutes(5));
        var session2 = new UserSession(Guid.NewGuid(), "token2", now + TimeSpan.FromMinutes(20));
        authenticator.EstablishUserSession(session1);

        timeProvider.Advance(TimeSpan.FromMinutes(4));
        authenticator.RefreshUserSession(session2);

        // Advance past session1's original expiry — session2 should still be active.
        timeProvider.Advance(TimeSpan.FromMinutes(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(authenticator.IsAuthenticated, Is.True);
            Assert.That(authenticator.UserSession, Is.EqualTo(session2));
        }
    }

    [Test]
    public void RefreshUserSessionShouldThrowArgumentNullExceptionWhenSessionIsNull() =>
        Assert.Throws<ArgumentNullException>(() => authenticator.RefreshUserSession(null));

    [Test]
    public void RefreshUserSessionShouldThrowObjectDisposedExceptionWhenDisposed()
    {
        var session = new UserSession(Guid.NewGuid(), "token", DateTimeOffset.UtcNow.AddMinutes(15));
        authenticator.Dispose();

        Assert.Throws<ObjectDisposedException>(() => authenticator.RefreshUserSession(session));
    }

    [Test]
    public void RefreshUserSessionShouldUpdateSessionWhenAuthenticated()
    {
        var session1 = new UserSession(Guid.NewGuid(), "token1", DateTimeOffset.UtcNow.AddMinutes(15));
        var session2 = new UserSession(Guid.NewGuid(), "token2", DateTimeOffset.UtcNow.AddMinutes(16));
        authenticator.EstablishUserSession(session1);

        authenticator.RefreshUserSession(session2);

        Assert.That(authenticator.UserSession, Is.EqualTo(session2));
    }

    [Test]
    public void SessionShouldBeInvalidatedAutomaticallyWhenTimerElapses()
    {
        var expiry = TimeSpan.FromMinutes(15);
        var session = new UserSession(Guid.NewGuid(), "token", timeProvider.GetUtcNow() + expiry);
        authenticator.EstablishUserSession(session);

        timeProvider.Advance(expiry);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(authenticator.IsAuthenticated, Is.False);
            Assert.That(authenticator.UserSession, Is.Null);
        }
    }

    [Test]
    public void SessionShouldFireInvalidatedEventWhenTimerElapses()
    {
        var expiry = TimeSpan.FromMinutes(15);
        var session = new UserSession(Guid.NewGuid(), "token", timeProvider.GetUtcNow() + expiry);
        authenticator.EstablishUserSession(session);

        int raisedCount = 0;
        authenticator.Invalidated += (_, _) => raisedCount++;

        timeProvider.Advance(expiry);

        Assert.That(raisedCount, Is.EqualTo(1));
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<UserSessionManager>>();
        timeProvider = new FakeTimeProvider();
        timeProvider.SetUtcNow(DateTimeOffset.UtcNow);
        authenticator = new UserSessionManager(logger, timeProvider);
    }

    [TearDown]
    public void TearDown() => authenticator.Dispose();

    [Test]
    public void UserSessionShouldBeNullByDefault() =>
        Assert.That(authenticator.UserSession, Is.Null);
}
