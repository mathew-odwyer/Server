namespace Winterhaven.API.Tests.Core.Application.Requests.Users.LogoutUser;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Exceptions;
using Winterhaven.API.Core.Application.Requests.Users.LogoutUser;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

[TestFixture]
internal sealed class LogoutUserRequestHandlerTests
{
    private LogoutUserRequestHandler handler;

    private ILogger<LogoutUserRequestHandler> logger;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private UserSessionToken userSessionToken;

    private Player player;

    private IActorContext actorContext;

    private IUserAccountRepository userAccountRepository;

    private Actor actor;

    private IUserSessionTokenRepository userSessionTokenRepository;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(null, this.unitOfWorkFactory, this.userSessionTokenRepository, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, this.unitOfWorkFactory, this.userSessionTokenRepository, null, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, null, this.userSessionTokenRepository, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesssionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, this.unitOfWorkFactory, null, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, this.unitOfWorkFactory, this.userSessionTokenRepository, this.actorContext, null));
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountRepositoryReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        this.userAccountRepository.GetByIdAsync(this.actor.Id).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldAccessActorContextWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        _ = this.actorContext.Received(1).Actor;
    }

    [Test]
    public async Task HandleShouldNotInvokeSaveAsyncWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        this.userSessionTokenRepository
            .GetActiveSessionAsync(this.userAccount.Id, default)
            .ReturnsNull();

        // Act
        try
        {
            await this.handler.Handle(request, default).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
        }

        // Assert
#pragma warning disable CS4014
        this.unitOfWork.DidNotReceive().SaveAsync(default);
#pragma warning restore CS4014
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.userAccountRepository.Received(1).GetByIdAsync(this.actor.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowConflictExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new LogoutUserRequest();

        this.unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<ConflictException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldSetIsRevokedToTrueWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        var request = new LogoutUserRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(this.userSessionToken.IsRevoked, Is.True);
    }

    [Test]
    public void HandleShouldThrowInvalidOperationExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        this.userSessionTokenRepository
            .GetActiveSessionAsync(this.userAccount.Id, default)
            .ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).GetActiveSessionAsync(this.userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<LogoutUserRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        this.actorContext = Substitute.For<IActorContext>();
        this.userAccountRepository = Substitute.For<IUserAccountRepository>();

        this.actor = new Actor()
        {
            Name = "User",
            Id = Guid.NewGuid(),
            Type = ActorType.User,
        };

        this.player = new Player()
        {
            Name = "username",
        };

        this.userAccount = new UserAccount()
        {
            Id = this.actor.Id,
            Username = "User",
            EmailAddress = "test@gmail.com",
            Player = this.player,
        };

        this.userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "HashedRefreshToken",
            UserAccount = this.userAccount,
            AccessTokenExpirationDate = DateTime.MinValue,
            RefreshTokenExpirationDate = DateTime.MaxValue,
        };

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);
        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);
        this.actorContext.Actor.Returns(this.actor);
        this.userAccountRepository.GetByIdAsync(this.actor.Id).Returns(this.userAccount);

        this.handler = new LogoutUserRequestHandler(
            this.logger,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository,
            this.actorContext,
            this.userAccountRepository);
    }
}