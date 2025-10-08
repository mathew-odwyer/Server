// <copyright file="DeletePlayerRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.DeletePlayer;

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
using Web.Application.Requests.Players.DeletePlayer;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class DeletePlayerRequestHandlerTests
{
    private DeletePlayerRequestHandler handler;

    private ILogger<DeletePlayerRequestHandler> logger;

    private Player player;

    private IPlayerRepository playerRepository;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new DeletePlayerRequestHandler(null, this.unitOfWorkFactory, this.playerRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new DeletePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new DeletePlayerRequestHandler(this.logger, null, this.playerRepository));
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowPlayerDeleteExceptionWhenDatabaseUpdateExceptionIsThrown()
    {
        // Arrange
        var request = new DeletePlayerRequest(
           UserAccountId: this.userAccount.Id,
           Name: this.player.Name);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateException>();

        // Act and assert
        Assert.ThrowsAsync<PlayerDeleteException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenPlayerExists()
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldSetPlayerIsDeletedToTrueWhenPlayerExists()
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(this.player.IsDeleted, Is.True);
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public void HandleShouldThrowEntityNotFoundExceptionWhenPlayerDoesNotExist()
    {
        // Arrange
        this.playerRepository.ClearSubstitute();

        var request = new DeletePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        // Act and Assert
        Assert.ThrowsAsync<EntityNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowForbiddenAccessExceptionWhenUserAccountIdDoesNotPlayersAccount()
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: "1",
            Name: this.player.Name);

        // Act and Assert
        Assert.ThrowsAsync<ForbiddenAccessException>(() => this.handler.Handle(request, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<DeletePlayerRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.playerRepository = Substitute.For<IPlayerRepository>();

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

        this.handler = new DeletePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.playerRepository);
    }
}
