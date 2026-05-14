namespace Winterhaven.API.Tests.Core.Application.Requests.Players.UpdatePlayer;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.Common.Exceptions;

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
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(null, this.unitOfWorkFactory, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, null, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.actorContext, null));
    }

    [Test]
    public async Task HandleShouldOnlyUpdateProvidedCoordinatesWhenPartialRequestIsGiven()
    {
        // Arrange
        double originalY = this.player.Y;

        var request = new UpdatePlayerRequest(X: 99, Y: null);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.player.X, Is.EqualTo(99));
            Assert.That(this.player.Y, Is.EqualTo(originalY));
        }
    }

    [Test]
    public async Task HandleShouldAccessActorContextWhenRequestIsNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(X: 0, Y: 0);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        _ = this.actorContext.Received(1).Actor;
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(X: 0, Y: 0);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.userAccountRepository.Received(1).GetByIdAsync(this.actor.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountRepositoryReturnsNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(X: 0, Y: 0);

        this.userAccountRepository.GetByIdAsync(this.actor.Id).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenPlayerIsFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            X: 0,
            Y: 0);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowPlayerUpdateExceptionWhenUnitOfWorkThrowsDatabaseUpdateException()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            X: 0,
            Y: 0);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<EntityPersistenceException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldRetainPlayerCoordinatesWhenCoordinatesAreNull()
    {
        // Arrange
        double originalX = this.player.X;
        double originalY = this.player.Y;

        var request = new UpdatePlayerRequest(X: null, Y: null);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.player.X, Is.EqualTo(originalX));
            Assert.That(this.player.Y, Is.EqualTo(originalY));
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
        await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.player.X, Is.EqualTo(request.X));
            Assert.That(this.player.Y, Is.EqualTo(request.Y));
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
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<UpdatePlayerRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.actorContext = Substitute.For<IActorContext>();
        this.userAccountRepository = Substitute.For<IUserAccountRepository>();

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.actor = new Actor()
        {
            Id = Guid.NewGuid(),
            Name = "Player",
            Type = ActorType.User,
        };

        this.player = new Player()
        {
            Name = "Player",
        };

        this.userAccount = new UserAccount()
        {
            Id = this.actor.Id,
            EmailAddress = "test@gmail.com",
            Username = this.player.Name,
            Player = this.player,
        };

        this.actorContext.Actor.Returns(this.actor);
        this.userAccountRepository.GetByIdAsync(this.actor.Id).Returns(this.userAccount);
        this.handler = new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.actorContext, this.userAccountRepository);
    }
}