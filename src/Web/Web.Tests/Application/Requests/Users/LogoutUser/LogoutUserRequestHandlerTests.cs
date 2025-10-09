// <copyright file="LogoutUserRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.LogoutUser;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Requests.Users.LogoutUser;
using Web.Domain.Entities.Users;

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
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(null, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, null, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesssionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LogoutUserRequestHandler(this.logger, this.unitOfWorkFactory, null));
    }

    [Test]
    public void HandleShouldThrowConflictExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new LogoutUserRequest(
            UserAccountId: this.userAccount.Id);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateConcurrencyException>();

        // Act and assert
        Assert.ThrowsAsync<ConflictException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeSaveAsyncWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        // Arrange
        var request = new LogoutUserRequest(
            UserAccountId: this.userAccount.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldSetExpirationDateToUtcNowWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        // Arrange
        var expected = DateTime.UtcNow;

        var request = new LogoutUserRequest(
            UserAccountId: this.userAccount.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(this.userSessionToken.ExpirationDate, Is.InRange(expected.AddSeconds(-1), expected.AddSeconds(1)));
    }

    [Test]
    public void HandleShouldThrowInvalidOperationExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new LogoutUserRequest(
            UserAccountId: this.userAccount.Id);

        this.userSessionTokenRepository.ClearSubstitute();

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LogoutUserRequest(
            UserAccountId: this.userAccount.Id);

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
        var request = new LogoutUserRequest(
            UserAccountId: this.userAccount.Id);

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

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<LogoutUserRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();

        this.userAccount = new UserAccount()
        {
            Id = "0",
            UserName = "User",
        };

        this.userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "HashedRefreshToken",
            SessionId = Guid.NewGuid(),
            UserAccountId = this.userAccount.Id,
            ExpirationDate = DateTime.MinValue,
        };

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);
        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);

        this.handler = new LogoutUserRequestHandler(
            this.logger,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository);
    }
}
