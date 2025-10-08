// <copyright file="UpdatePlayerRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.UpdatePlayer;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Web.Application.Contexts;
using Web.Application.Contexts.Players;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Exceptions.Players;
using Web.Application.Requests.Players.UpdatePlayer;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class UpdatePlayerRequestHandlerTests
{
    private UpdatePlayerRequestHandler handler;

    private ILogger<UpdatePlayerRequestHandler> logger;

    private Player player;

    private IPlayerRepository playerRepository;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(null, this.unitOfWorkFactory, this.playerRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, null, this.playerRepository));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public void HanldeShouldThrowForbiddenAccessExceptionWhenUserAccountIdDoesNotMatchPlayerUserAccount()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: "1",
            Name: "Player",
            X: 0,
            Y: 0);

        // Act and assert
        Assert.ThrowsAsync<ForbiddenAccessException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenPlayerIsFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name,
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
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name,
            X: 0,
            Y: 0);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateException>();

        // Act and assert
        Assert.ThrowsAsync<PlayerUpdateException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldSetPlayerCoordinatesToSameCoordinatesWhenWhenCoordinatesAreNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name,
            X: null,
            Y: null);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(this.player.X, Is.EqualTo(this.player.X));
            Assert.That(this.player.Y, Is.EqualTo(this.player.Y));
        });
    }

    [Test]
    public async Task HandleShouldSetPlayerCoordinatesToRequestCoordinatesWhenCoordinatesAreNotNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name,
            X: 1,
            Y: 2);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(this.player.X, Is.EqualTo(request.X));
            Assert.That(this.player.Y, Is.EqualTo(request.Y));
        });
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenPlayerIsFound()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name,
            X: 0,
            Y: 0);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public void HandleShouldThrowEntityNotFoundExceptionWhenGetByPlayerNameAsyncReturnsNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: "Player",
            X: 0,
            Y: 0);

        this.playerRepository.ClearSubstitute();

        // Act and assert
        Assert.ThrowsAsync<EntityNotFoundException>(() => this.handler.Handle(request, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<UpdatePlayerRequestHandler>>();
        this.playerRepository = Substitute.For<IPlayerRepository>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.userAccount = new UserAccount()
        {
            Id = "0",
        };

        this.player = new Player()
        {
            Name = "Player",
            UserAccountId = this.userAccount.Id,
        };

        this.playerRepository.GetPlayerByNameAsync(this.player.Name, default).Returns(this.player);

        this.handler = new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.playerRepository);
    }
}
