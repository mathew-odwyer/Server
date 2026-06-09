using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Users.RefreshToken;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Core.Domain.ValueObjects.Users;
using Winterhaven.Common.Exceptions;

namespace Winterhaven.API.Tests.Core.Application.Requests.Users.RefreshToken;

[TestFixture]
internal sealed class RefreshTokenRequestHandlerTests
{
    private RefreshTokenRequestHandler handler;

    private ILogger<RefreshTokenRequestHandler> logger;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private UserToken userToken;

    private UserSessionToken userSessionToken;

    private IUserSessionTokenRepository userSessionTokenRepository;

    private IActorContext actorContext;

    private IUserAccountRepository userAccountRepository;

    private Player player;

    private ISecureTokenFactory secureTokenFactory;

    private ISecureTokenHasher secureTokenHasher;

    private Actor actor;

    [Test]
    public async Task HandleShouldNotRevokeNewSessionWhenRefreshIsValid()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: userToken.RefreshToken);

        UserSessionToken addedSession = null;
        userSessionTokenRepository
            .When(x => x.AddAsync(Arg.Any<UserSessionToken>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => addedSession = callInfo.Arg<UserSessionToken>());

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(addedSession!.IsRevoked, Is.False);
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountRepositoryReturnsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: userToken.RefreshToken);

        userAccountRepository.GetByIdAsync(actor.Id).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await userAccountRepository.Received(1).GetByIdAsync(actor.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task HandleShouldAccessActorContextWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        _ = actorContext.Received(1).Actor;
    }

    [Test]
    public async Task HandleShouldInvokeHashRefreshTokenWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        secureTokenHasher.Received(2).HashSecureToken(request.RefreshToken);
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        secureTokenFactory.Received(1).GenerateUserToken(Arg.Any<UserTokenParameters>());
    }

    [Test]
    public async Task HandleShouldSetIsRevokedToTrueWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(userSessionToken.IsRevoked, Is.True);
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsExpiredRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = this.userSessionToken.HashedRefreshToken,
            UserAccount = userAccount,
            RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(-1),
        };

        userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsIncorrectHashedRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "Test",
            UserAccount = userAccount,
        };

        userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, default).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userSessionTokenRepository.Received(1).GetActiveSessionAsync(userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(null, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(logger, secureTokenFactory, secureTokenHasher, null, userSessionTokenRepository, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(logger, null, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionTokenRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(logger, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, null, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenHasherIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(logger, secureTokenFactory, null, unitOfWorkFactory, userSessionTokenRepository, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(logger, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository, null, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(logger, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository, actorContext, null));

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, default));

    [Test]
    public async Task HandleShouldReturnNewAccessAndRefreshTokenWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        var response = await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.AccessToken, Is.EqualTo(userToken.AccessToken));
            Assert.That(response.RefreshToken, Is.EqualTo(userToken.RefreshToken));
        }
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUserSessionTokenRepositoryAddAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userSessionTokenRepository.Received(1).AddAsync(Arg.Any<UserSessionToken>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldAddUserSessionTokenWithExpectedValuesWhenRefreshIsValid()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: userToken.RefreshToken);

        UserSessionToken addedSession = null;

        userSessionTokenRepository
            .When(x => x.AddAsync(Arg.Any<UserSessionToken>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => addedSession = callInfo.Arg<UserSessionToken>());

        string expectedHashedRefreshToken = userSessionToken.HashedRefreshToken;
        const double expectedExpiryMinutes = 15;

        var utcNow = DateTime.UtcNow;

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(addedSession, Is.Not.Null, "Expected a UserSessionToken to be added.");
            Assert.That(addedSession!.UserAccount, Is.SameAs(userAccount), "UserAccountId does not match.");
            Assert.That(addedSession.HashedRefreshToken, Is.EqualTo(expectedHashedRefreshToken), "HashedRefreshToken does not match.");
            Assert.That(
                addedSession.AccessTokenExpirationDate,
                Is.InRange(utcNow.AddMinutes(expectedExpiryMinutes - 1), utcNow.AddMinutes(expectedExpiryMinutes + 1)),
                "ExpirationDate is not within the expected range.");
        }
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<RefreshTokenRequestHandler>>();
        unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        secureTokenFactory = Substitute.For<ISecureTokenFactory>();
        secureTokenHasher = Substitute.For<ISecureTokenHasher>();
        userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        actorContext = Substitute.For<IActorContext>();
        userAccountRepository = Substitute.For<IUserAccountRepository>();

        userToken = new UserToken(
            AccessToken: "accessToken",
            RefreshToken: "refreshToken",
            AccessTokenExpiryDate: DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiryDate: DateTime.UtcNow.AddDays(7));

        player = new Player()
        {
            Name = "username",
        };

        userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = "username",
            EmailAddress = "test@email.com",
            Player = player,
        };

        userSessionToken = new UserSessionToken()
        {
            UserAccount = userAccount,
            HashedRefreshToken = "HashedRefreshToken",
            AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
        };

        actor = new Actor()
        {
            Id = userAccount.Id,
            Name = userAccount.Username,
            Type = ActorType.User,
        };

        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);

        userAccountRepository.GetByIdAsync(actor.Id).Returns(userAccount);
        secureTokenFactory.GenerateUserToken(Arg.Any<UserTokenParameters>()).Returns(userToken);
        secureTokenHasher.HashSecureToken(userToken.RefreshToken).Returns(userSessionToken.HashedRefreshToken);
        userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, default).Returns(userSessionToken);
        actorContext.Actor.Returns(actor);

        handler = new RefreshTokenRequestHandler(
            logger,
            secureTokenFactory,
            secureTokenHasher,
            unitOfWorkFactory,
            userSessionTokenRepository,
            actorContext,
            userAccountRepository);
    }
}
