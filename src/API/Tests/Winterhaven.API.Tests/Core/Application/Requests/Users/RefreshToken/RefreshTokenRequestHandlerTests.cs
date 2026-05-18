namespace Winterhaven.API.Tests.Core.Application.Requests.Users.RefreshToken;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Users.RefreshToken;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Core.Domain.ValueObjects.Users;

[TestFixture]
internal sealed class RefreshTokenRequestHandlerTests
{
    private RefreshTokenRequestHandler handler;

    private UserTokenParameters userTokenParameters;

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
        var request = new RefreshTokenRequest(RefreshToken: this.userToken.RefreshToken);

        UserSessionToken addedSession = null;
        this.userSessionTokenRepository
            .When(x => x.AddAsync(Arg.Any<UserSessionToken>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => addedSession = callInfo.Arg<UserSessionToken>());

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(addedSession!.IsRevoked, Is.False);
    }

    [Test]
    public async Task HandleShouldReturnCorrectExpirationSecondsWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: this.userToken.RefreshToken);
        var before = DateTime.UtcNow;

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        var after = DateTime.UtcNow;

        // Assert — should be roughly 15 minutes worth of seconds
        Assert.That(response.ExpirationSeconds, Is.InRange(
            this.userToken.AccessTokenExpiryDate.Subtract(after).TotalSeconds,
            this.userToken.AccessTokenExpiryDate.Subtract(before).TotalSeconds));
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountRepositoryReturnsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: this.userToken.RefreshToken);

        this.userAccountRepository.GetByIdAsync(this.actor.Id).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.userAccountRepository.Received(1).GetByIdAsync(this.actor.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task HandleShouldAccessActorContextWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        _ = this.actorContext.Received(1).Actor;
    }

    [Test]
    public async Task HandleShouldInvokeHashRefreshTokenWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.secureTokenHasher.Received(2).HashSecureToken(request.RefreshToken);
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.secureTokenFactory.Received(1).GenerateUserToken(Arg.Any<UserTokenParameters>());
    }

    [Test]
    public async Task HandleShouldSetIsRevokedToTrueWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(this.userSessionToken.IsRevoked, Is.True);
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsExpiredRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = this.userSessionToken.HashedRefreshToken,
            UserAccount = this.userAccount,
            RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(-1),
        };

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsIncorrectHashedRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "Test",
            UserAccount = this.userAccount,
        };

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).GetActiveSessionAsync(this.userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(null, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.secureTokenFactory, this.secureTokenHasher, null, this.userSessionTokenRepository, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, null, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, null, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenHasherIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.secureTokenFactory, null, this.unitOfWorkFactory, this.userSessionTokenRepository, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, null, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, this.actorContext, null));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public async Task HandleShouldReturnNewAccessAndRefreshTokenWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.AccessToken, Is.EqualTo(this.userToken.AccessToken));
            Assert.That(response.RefreshToken, Is.EqualTo(this.userToken.RefreshToken));
        }
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUserSessionTokenRepositoryAddAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).AddAsync(Arg.Any<UserSessionToken>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldAddUserSessionTokenWithExpectedValuesWhenRefreshIsValid()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.userToken.RefreshToken);

        UserSessionToken addedSession = null;

        this.userSessionTokenRepository
            .When(x => x.AddAsync(Arg.Any<UserSessionToken>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => addedSession = callInfo.Arg<UserSessionToken>());

        string expectedHashedRefreshToken = this.userSessionToken.HashedRefreshToken;
        const double expectedExpiryMinutes = 15;

        var utcNow = DateTime.UtcNow;

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(addedSession, Is.Not.Null, "Expected a UserSessionToken to be added.");
            Assert.That(addedSession!.UserAccount, Is.SameAs(this.userAccount), "UserAccountId does not match.");
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
        this.logger = Substitute.For<ILogger<RefreshTokenRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.secureTokenFactory = Substitute.For<ISecureTokenFactory>();
        this.secureTokenHasher = Substitute.For<ISecureTokenHasher>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.actorContext = Substitute.For<IActorContext>();
        this.userAccountRepository = Substitute.For<IUserAccountRepository>();

        this.userToken = new UserToken(
            AccessToken: "accessToken",
            RefreshToken: "refreshToken",
            AccessTokenExpiryDate: DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiryDate: DateTime.UtcNow.AddDays(7));

        this.player = new Player()
        {
            Name = "username",
        };

        this.userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = "username",
            EmailAddress = "test@email.com",
            Player = this.player,
        };

        this.userSessionToken = new UserSessionToken()
        {
            UserAccount = this.userAccount,
            HashedRefreshToken = "HashedRefreshToken",
            AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
        };

        this.actor = new Actor()
        {
            Id = this.userAccount.Id,
            Name = this.userAccount.Username,
            Type = ActorType.User,
        };

        this.userTokenParameters = new UserTokenParameters(
            UserAccountId: this.userAccount.Id,
            Username: this.userAccount.Username);

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.userAccountRepository.GetByIdAsync(this.actor.Id).Returns(this.userAccount);
        this.secureTokenFactory.GenerateUserToken(Arg.Any<UserTokenParameters>()).Returns(this.userToken);
        this.secureTokenHasher.HashSecureToken(this.userToken.RefreshToken).Returns(this.userSessionToken.HashedRefreshToken);
        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);
        this.actorContext.Actor.Returns(this.actor);

        this.handler = new RefreshTokenRequestHandler(
            this.logger,
            this.secureTokenFactory,
            this.secureTokenHasher,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository,
            this.actorContext,
            this.userAccountRepository);
    }
}