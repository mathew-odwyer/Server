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
using Winterhaven.API.Core.Domain.Entities.Players;
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

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(null, unitOfWorkFactory, playerRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(logger, unitOfWorkFactory, null));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(logger, null, playerRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull() => Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(logger, unitOfWorkFactory, null));

    [Test]
    public async Task HandleShouldOnlyUpdateProvidedCoordinatesWhenPartialRequestIsGiven()
    {
        // Arrange
        double originalY = player.Y;

        var request = new UpdatePlayerRequest(PlayerId: player.Id, X: 99, Y: null);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(player.X, Is.EqualTo(99));
            Assert.That(player.Y, Is.EqualTo(originalY));
        }
    }

    [Test]
    public async Task HandleShouldInvokeGetByIdAsyncWithActorIdWhenRequestIsNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(PlayerId: player.Id, X: 0, Y: 0);

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await playerRepository.Received(1).GetByIdAsync(player.Id, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenPlayerRepositoryyReturnsNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(PlayerId: player.Id, X: 0, Y: 0);

        playerRepository.GetByIdAsync(player.Id).ReturnsNull();

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
            PlayerId: player.Id,
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
            PlayerId: player.Id,
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

        var request = new UpdatePlayerRequest(PlayerId: player.Id, X: null, Y: null);

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
            player.Id,
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
            PlayerId: player.Id,
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
        playerRepository = Substitute.For<IPlayerRepository>();

        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);

        player = new Player()
        {
            Id = Guid.NewGuid(),
            Name = "Player",
        };

        playerRepository.GetByIdAsync(player.Id).Returns(player);

        handler = new UpdatePlayerRequestHandler(logger, unitOfWorkFactory, playerRepository);
    }
}
