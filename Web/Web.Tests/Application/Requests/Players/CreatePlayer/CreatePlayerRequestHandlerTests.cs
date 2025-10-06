// <copyright file="CreatePlayerRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.CreatePlayer;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Web.Application.Contexts;
using Web.Application.Contexts.Players;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Exceptions.Players;
using Web.Application.Requests.Players.CreatePlayer;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class CreatePlayerRequestHandlerTests
{
    private CreatePlayerRequestHandler handler;

    private ILogger<CreatePlayerRequestHandler> logger;

    private Player player;

    private IPlayerRepository playerRepository;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private IUserAccountRepository userAccountRepository;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new CreatePlayerRequestHandler(null, this.unitOfWorkFactory, this.playerRepository, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new CreatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, null, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new CreatePlayerRequestHandler(this.logger, null, this.playerRepository, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new CreatePlayerRequestHandler(this.logger, this.unitOfWorkFactory, this.playerRepository, null));
    }

    [Test]
    public async Task HandleShouldAddPlayerToRepositoryWhenPlayerDoesNotExist()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        this.userAccountRepository.GetByIdAsync(this.userAccount.Id, Arg.Any<CancellationToken>()).Returns(this.userAccount);
        this.playerRepository.IsPlayerExists(this.player.Name, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        this.playerRepository.Received(1).AddAsync(
            Arg.Is<Player>(p =>
                p.Name == this.player.Name &&
                p.UserAccountId == this.player.UserAccountId),
            Arg.Any<CancellationToken>());

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldCreateUnitOfWorkWhenPlayerDoesNotExist()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        this.userAccountRepository.GetByIdAsync(this.userAccount.Id, Arg.Any<CancellationToken>()).Returns(this.userAccount);
        this.playerRepository.IsPlayerExists(this.player.Name, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public async Task HandleShouldSaveChangesWhenPlayerDoesNotExist()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        this.userAccountRepository.GetByIdAsync(this.userAccount.Id, Arg.Any<CancellationToken>()).Returns(this.userAccount);
        this.playerRepository.IsPlayerExists(this.player.Name, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        this.unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public void HandleShouldThrowConflictExceptionWhenPlayerAlreadyExists()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        this.userAccountRepository.GetByIdAsync(this.userAccount.Id, Arg.Any<CancellationToken>()).Returns(this.userAccount);
        this.playerRepository.IsPlayerExists(this.player.Name, Arg.Any<CancellationToken>()).Returns(true);

        // Act and assert
        Assert.ThrowsAsync<ConflictException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowEntityNotFoundExceptionWhenUserAccountNotFound()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: null,
            Name: "player");

        // Act and assert
        Assert.ThrowsAsync<EntityNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowPlayerCreateExceptionWhenUnitOfWorkThrowsDatabaseUpdateException()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: this.userAccount.Id,
            Name: this.player.Name);

        this.userAccountRepository.GetByIdAsync(this.userAccount.Id, Arg.Any<CancellationToken>()).Returns(this.userAccount);
        this.playerRepository.IsPlayerExists(this.player.Name, Arg.Any<CancellationToken>()).Returns(false);
        this.unitOfWork.SaveAsync(default).Throws<DatabaseUpdateException>();

        // Act and assert
        Assert.ThrowsAsync<PlayerCreateException>(() => this.handler.Handle(request, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<CreatePlayerRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.playerRepository = Substitute.For<IPlayerRepository>();
        this.userAccountRepository = Substitute.For<IUserAccountRepository>();
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

        this.handler = new CreatePlayerRequestHandler(
            this.logger,
            this.unitOfWorkFactory,
            this.playerRepository,
            this.userAccountRepository);
    }
}
