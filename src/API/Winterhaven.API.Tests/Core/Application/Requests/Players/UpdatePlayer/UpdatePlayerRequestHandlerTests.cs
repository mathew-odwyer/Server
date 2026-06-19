using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Players;
using Winterhaven.API.Core.Application.Work.Rooms;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Rooms;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Tests.Core.Application.Requests.Players.UpdatePlayer;

[TestFixture]
internal sealed class UpdatePlayerRequestHandlerTests
{
    private UpdatePlayerRequestHandler handler;

    private ILogger<UpdatePlayerRequestHandler> logger;

    private Player player;

    private IPlayerRepository playerRepository;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private IRoomRepository roomRepository;

    [Test]
    public async Task HandleShouldPassCancellationTokenToRoomRepositoryWhenInvoked()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            PlayerId: this.player.Id,
            X: 0,
            Y: 0,
            RoomId: this.room.Id);

        var cancellationToken = new CancellationToken(false);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.roomRepository.Received(1).GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);
    }

    [Test]
    public async Task HandleShouldPassCancellationTokenToPlayerRepositoryWhenInvoked()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            PlayerId: this.player.Id,
            X: 0,
            Y: 0,
            RoomId: this.room.Id);

        var cancellationToken = new CancellationToken(false);

        // Act
        await this.handler.Handle(request, cancellationToken).ConfigureAwait(false);

        // Assert
        await this.playerRepository.Received(1).GetByIdAsync(request.PlayerId, cancellationToken).ConfigureAwait(false);
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(null, this.unitOfWorkFactory, this.playerRepository, this.roomRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null, this.roomRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, null, this.playerRepository, this.roomRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null, this.roomRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenRoomRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.playerRepository, null));
    }

    [Test]
    public async Task HandleShouldOnlyUpdateProvidedCoordinatesWhenPartialRequestIsGiven()
    {
        // Arrange
        double originalY = this.player.Y;

        var request = new UpdatePlayerRequest(PlayerId: this.player.Id, X: 99, Y: null, RoomId: this.room.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.player.X, Is.EqualTo(99));
            Assert.That(this.player.Y, Is.EqualTo(originalY));
        }
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(PlayerId: this.player.Id, X: 0, Y: 0, RoomId: this.room.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.playerRepository.Received(1).GetByIdAsync(this.player.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenPlayerRepositoryyReturnsNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(PlayerId: this.player.Id, X: 0, Y: 0, RoomId: this.room.Id);

        this.playerRepository.GetByIdAsync(this.player.Id).ReturnsNull();

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
            PlayerId: this.player.Id,
            X: 0,
            Y: 0,
            RoomId: this.room.Id);

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
            PlayerId: this.player.Id,
            X: 0,
            Y: 0,
            RoomId: this.room.Id);

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

        var request = new UpdatePlayerRequest(PlayerId: this.player.Id, X: null, Y: null, RoomId: this.room.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.player.X, Is.EqualTo(originalX));
            Assert.That(this.player.Y, Is.EqualTo(originalY));
        }
    }

    [Test]
    public async Task HandleShouldSetPlayerLastKnownRoomWhenRoomIsNotNull()
    {
        // Arrange
        var newRoom = new Room()
        {
            Id = Guid.NewGuid(),
            MapFilePath = "map/to/new/ROOM",
            MapName = "NEW ROOM",
        };

        var request = new UpdatePlayerRequest(
            this.player.Id,
            X: null,
            Y: null,
            RoomId: newRoom.Id);

        this.roomRepository.GetByIdAsync(newRoom.Id, Arg.Any<CancellationToken>()).Returns(newRoom);

        // Act
        await this.handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.player.LastKnownRoom, Is.EqualTo(newRoom));
            Assert.That(this.player.LastKnownRoom.Id, Is.EqualTo(newRoom.Id));
            Assert.That(this.player.LastKnownRoom.MapName, Is.EqualTo(newRoom.MapName));
            Assert.That(this.player.LastKnownRoom.MapFilePath, Is.EqualTo(newRoom.MapFilePath));
        }
    }

    [Test]
    public async Task HandleShouldSetPlayerCoordinatesToRequestCoordinatesWhenCoordinatesAreNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            this.player.Id,
            X: 1,
            Y: 2,
            RoomId: this.room.Id);

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
    public void HandleShouldThrowResourceNotFoundExceptionWhenRoomIsNotFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            PlayerId: this.player.Id,
            X: 0,
            Y: 0,
            RoomId: Guid.NewGuid());

        this.roomRepository.GetByIdAsync(request.PlayerId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenPlayerIsFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            PlayerId: this.player.Id,
            X: 0,
            Y: 0,
            RoomId: this.room.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private Room room;

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<UpdatePlayerRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.playerRepository = Substitute.For<IPlayerRepository>();
        this.roomRepository = Substitute.For<IRoomRepository>();

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.player = new Player()
        {
            Id = Guid.NewGuid(),
            Name = "Player",
        };

        this.room = new Room()
        {
            Id = Guid.NewGuid(),
            MapName = "ROOM",
            MapFilePath = "some/path/ROOM",
        };

        this.playerRepository.GetByIdAsync(this.player.Id).Returns(this.player);
        this.roomRepository.GetByIdAsync(this.room.Id).Returns(this.room);

        this.handler = new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.playerRepository, this.roomRepository);
    }
}
