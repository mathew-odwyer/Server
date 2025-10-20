// <copyright file="LogoutUserRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Tests.Application.Requests.Users.LogoutUser;

using Mantanimus.Core.Application.Contexts.Users;
using Mantanimus.Core.Application.Exceptions;
using Mantanimus.Core.Application.Exceptions.Database;
using Mantanimus.Core.Application.Requests.Users.LogoutUser;
using Mantanimus.Core.Application.Work;
using Mantanimus.Core.Application.Work.Users;
using Mantanimus.Core.Domain.Entities.Players;
using Mantanimus.Core.Domain.Entities.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

[TestFixture]
internal sealed class LogoutUserRequestHandlerTests
{
    private LogoutUserRequestHandler handler;

    private ILogger<LogoutUserRequestHandler> logger;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private UserSessionToken userSessionToken;

    private IUserSessionTokenRepository userSessionTokenRepository;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(null, this.unitOfWorkFactory, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(null, this.unitOfWorkFactory, this.userSessionTokenRepository, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, null, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesssionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, this.unitOfWorkFactory, null, this.userAccountContext));
    }

    [Test]
    public void HandleShouldThrowConflictExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new LogoutUserRequest();

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateException>();

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
        // Arrange
        _ = DateTime.UtcNow;

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

        this.userSessionTokenRepository.ClearSubstitute();

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

    private Player player;

    private IUserAccountContext userAccountContext;

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<LogoutUserRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        this.userAccountContext = Substitute.For<IUserAccountContext>();

        this.player = new Player()
        {
            Name = "username",
        };

        this.userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = "User",
            EmailAddress = "test@gmail.com",
            Player = this.player,
        };

        this.userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "HashedRefreshToken",
            UserAccount = this.userAccount,
            ExpirationDate = DateTime.MinValue,
        };

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);
        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);
        this.userAccountContext.User.Returns(this.userAccount);

        this.handler = new LogoutUserRequestHandler(
            this.logger,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository,
            this.userAccountContext);
    }
}
