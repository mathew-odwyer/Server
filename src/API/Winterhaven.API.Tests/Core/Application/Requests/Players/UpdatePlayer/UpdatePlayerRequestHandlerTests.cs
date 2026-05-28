using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Tests.Core.Application.Requests.Players.UpdatePlayer;

[TestFixture]
internal sealed class UpdatePlayerRequestHandlerTests
{
    private UpdatePlayerRequestHandler handler;

    private ILogger<UpdatePlayerRequestHandler> logger;

    private Player player;

    private IActorContext actorContext;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private IUserAccountRepository userAccountRepository;

    private Actor actor;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(null, unitOfWorkFactory, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(logger, unitOfWorkFactory, null, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(logger, null, actorContext, userAccountRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull() => Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(logger, unitOfWorkFactory, actorContext, null));

    [Test]
    public async Task HandleShouldOnlyUpdateProvidedCoordinatesWhenPartialRequestIsGiven()
    {
        // Arrange
        double originalY = player.Y;

        var request = new UpdatePlayerRequest(X: 99, Y: null);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.X, Is.EqualTo(99));
            Assert.That(player.Y, Is.EqualTo(originalY));
        }
    }

    [Test]
    public async Task HandleShouldAccessActorContextWhenRequestIsNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(X: 0, Y: 0);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        _ = actorContext.Received(1).Actor;
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(X: 0, Y: 0);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await userAccountRepository.Received(1).GetByIdAsync(actor.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountRepositoryReturnsNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(X: 0, Y: 0);

        userAccountRepository.GetByIdAsync(actor.Id).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, default));

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenPlayerIsFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            X: 0,
            Y: 0);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowPlayerUpdateExceptionWhenUnitOfWorkThrowsDatabaseUpdateException()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            X: 0,
            Y: 0);

        unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<EntityPersistenceException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldRetainPlayerCoordinatesWhenCoordinatesAreNull()
    {
        // Arrange
        double originalX = player.X;
        double originalY = player.Y;

        var request = new UpdatePlayerRequest(X: null, Y: null);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.X, Is.EqualTo(originalX));
            Assert.That(player.Y, Is.EqualTo(originalY));
        }
    }

    [Test]
    public async Task HandleShouldSetPlayerCoordinatesToRequestCoordinatesWhenCoordinatesAreNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            X: 1,
            Y: 2);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(player.X, Is.EqualTo(request.X));
            Assert.That(player.Y, Is.EqualTo(request.Y));
        }
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenPlayerIsFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            X: 0,
            Y: 0);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<UpdatePlayerRequestHandler>>();
        unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        actorContext = Substitute.For<IActorContext>();
        userAccountRepository = Substitute.For<IUserAccountRepository>();

        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);

        actor = new Actor()
        {
            Id = Guid.NewGuid(),
            Name = "Player",
            Type = ActorType.User,
        };

        player = new Player()
        {
            Name = "Player",
        };

        userAccount = new UserAccount()
        {
            Id = actor.Id,
            EmailAddress = "test@gmail.com",
            Username = player.Name,
            Player = player,
        };

        actorContext.Actor.Returns(actor);
        userAccountRepository.GetByIdAsync(actor.Id).Returns(userAccount);
        handler = new UpdatePlayerRequestHandler(logger, unitOfWorkFactory, actorContext, userAccountRepository);
    }
}