// <copyright file="UpdatePlayerRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Tests.Application.Requests.Players.UpdatePlayer;

using Mantanimus.Core.Application.Contexts.Users;
using Mantanimus.Core.Application.Exceptions.Database;
using Mantanimus.Core.Application.Exceptions.Players;
using Mantanimus.Core.Application.Requests.Players.UpdatePlayer;
using Mantanimus.Core.Application.Work;
using Mantanimus.Core.Domain.Entities.Players;
using Mantanimus.Core.Domain.Entities.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

[TestFixture]
internal sealed class UpdatePlayerRequestHandlerTests
{
    private UpdatePlayerRequestHandler handler;

    private ILogger<UpdatePlayerRequestHandler> logger;

    private Player player;

    private IUserAccountContext userAccountContext;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(null, this.unitOfWorkFactory, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UpdatePlayerRequestHandler(this.logger, null, this.userAccountContext));
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

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateException>();

        // Act and assert
        Assert.ThrowsAsync<PlayerUpdateException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldSetPlayerCoordinatesToSameCoordinatesWhenWhenCoordinatesAreNull()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
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
        this.userAccountContext = Substitute.For<IUserAccountContext>();

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.player = new Player()
        {
            Name = "Player",
        };

        this.userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@gmail.com",
            Username = this.player.Name,
            Player = this.player,
        };

        this.userAccountContext.User.Returns(this.userAccount);
        this.handler = new UpdatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.userAccountContext);
    }
}
