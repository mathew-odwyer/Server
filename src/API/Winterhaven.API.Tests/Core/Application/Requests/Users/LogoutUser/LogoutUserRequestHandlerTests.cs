using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Users.LogoutUser;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Tests.Core.Application.Requests.Users.LogoutUser;

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
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(null, unitOfWorkFactory, userSessionTokenRepository, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(logger, unitOfWorkFactory, userSessionTokenRepository, null, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(logger, null, userSessionTokenRepository, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesssionTokenRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(logger, unitOfWorkFactory, null, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull() => Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(logger, unitOfWorkFactory, userSessionTokenRepository, actorContext, null));

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountRepositoryReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        userAccountRepository.GetByIdAsync(actor.Id).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldAccessActorContextWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        _ = actorContext.Received(1).Actor;
    }

    [Test]
    public async Task HandleShouldNotInvokeSaveAsyncWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        userSessionTokenRepository
            .GetActiveSessionAsync(userAccount.Id, default)
            .ReturnsNull();

        // Act
        try
        {
            await handler.Handle(request, default).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
        }

        // Assert
#pragma warning disable CS4014
        unitOfWork.DidNotReceive().SaveAsync(default);
#pragma warning restore CS4014
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await userAccountRepository.Received(1).GetByIdAsync(actor.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowConflictExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new LogoutUserRequest();

        unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<ConflictException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldSetIsRevokedToTrueWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        var request = new LogoutUserRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(userSessionToken.IsRevoked, Is.True);
    }

    [Test]
    public void HandleShouldThrowInvalidOperationExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        userSessionTokenRepository
            .GetActiveSessionAsync(userAccount.Id, default)
            .ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userSessionTokenRepository.Received(1).GetActiveSessionAsync(userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, default));

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<LogoutUserRequestHandler>>();
        unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        actorContext = Substitute.For<IActorContext>();
        userAccountRepository = Substitute.For<IUserAccountRepository>();

        actor = new Actor()
        {
            Name = "User",
            Id = Guid.NewGuid(),
            Type = ActorType.User,
        };

        player = new Player()
        {
            Name = "username",
        };

        userAccount = new UserAccount()
        {
            Id = actor.Id,
            Username = "User",
            EmailAddress = "test@gmail.com",
            Player = player,
        };

        userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "HashedRefreshToken",
            UserAccount = userAccount,
            AccessTokenExpirationDate = DateTime.MinValue,
            RefreshTokenExpirationDate = DateTime.MaxValue,
        };

        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);
        userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, default).Returns(userSessionToken);
        actorContext.Actor.Returns(actor);
        userAccountRepository.GetByIdAsync(actor.Id).Returns(userAccount);

        handler = new LogoutUserRequestHandler(
            logger,
            unitOfWorkFactory,
            userSessionTokenRepository,
            actorContext,
            userAccountRepository);
    }
}