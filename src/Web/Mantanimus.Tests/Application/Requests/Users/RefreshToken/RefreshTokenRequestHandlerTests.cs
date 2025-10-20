// <copyright file="RefreshTokenRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Tests.Application.Requests.Users.RefreshToken;

using Mantanimus.Core.Application.Contexts.Users;
using Mantanimus.Core.Application.Exceptions;
using Mantanimus.Core.Application.Exceptions.Database;
using Mantanimus.Core.Application.Options.Security;
using Mantanimus.Core.Application.Requests.Users.RefreshToken;
using Mantanimus.Core.Application.Services.Security;
using Mantanimus.Core.Application.Work;
using Mantanimus.Core.Application.Work.Users;
using Mantanimus.Core.Domain.Entities.Players;
using Mantanimus.Core.Domain.Entities.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

[TestFixture]
internal sealed class RefreshTokenRequestHandlerTests
{
    private RefreshTokenRequestHandler handler;

    private JwtToken jwtToken;

    private ILogger<RefreshTokenRequestHandler> logger;

    private IOptions<JwtOptions> options;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private UserSessionToken userSessionToken;

    private IUserSessionTokenRepository userSessionTokenRepository;

    [Test]
    public async Task HandleShouldInvokeHashRefreshTokenWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.secureTokenHasher.Received(2).HashSecureToken(request.RefreshToken);
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.secureTokenFactory.Received(1).GenerateJwt(Arg.Any<JwtParameters>());
    }

    [Test]
    public async Task HandleShouldSetIsRevokedToTrueWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        // Arrange
        _ = DateTime.UtcNow;

        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(this.userSessionToken.IsRevoked, Is.True);
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsExpiredRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = this.userSessionToken.HashedRefreshToken,
            UserAccount = this.userAccount,
            CreatedOn = DateTime.UtcNow.AddDays(-100),
        };

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsIncorrectHashedRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "Test",
            UserAccount = this.userAccount,
        };

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).GetActiveSessionAsync(this.userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(null, this.options, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, null, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, this.secureTokenFactory, this.secureTokenHasher, null, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, null, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, null, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenHasherIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, this.secureTokenFactory, null, this.unitOfWorkFactory, this.userSessionTokenRepository, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository, null));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public async Task HandleShouldReturnNewAccessAndRefreshTokenWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(response.AccessToken, Is.EqualTo(this.jwtToken.AccessToken));
            Assert.That(response.RefreshToken, Is.EqualTo(this.jwtToken.RefreshToken));
        });
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenSaveAsyncThrowsDatabaseUpdateConcurrencyException()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateException>();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUserSessionTokenRepositoryAddAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).AddAsync(Arg.Any<UserSessionToken>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldAddUserSessionTokenWithExpectedValuesWhenRefreshIsValid()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: this.jwtToken.RefreshToken);

        UserSessionToken addedSession = null;

        this.userSessionTokenRepository
            .When(x => x.AddAsync(Arg.Any<UserSessionToken>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => addedSession = callInfo.Arg<UserSessionToken>());

        string expectedHashedRefreshToken = this.userSessionToken.HashedRefreshToken;
        double expectedExpiryMinutes = this.options.Value.AccessTokenExpiryMinutes;

        var utcNow = DateTime.UtcNow;

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addedSession, Is.Not.Null, "Expected a UserSessionToken to be added.");
            Assert.That(addedSession!.UserAccount, Is.SameAs(this.userAccount), "UserAccountId does not match.");
            Assert.That(addedSession.HashedRefreshToken, Is.EqualTo(expectedHashedRefreshToken), "HashedRefreshToken does not match.");
            Assert.That(
                addedSession.ExpirationDate,
                Is.InRange(utcNow.AddMinutes(expectedExpiryMinutes - 1), utcNow.AddMinutes(expectedExpiryMinutes + 1)),
                "ExpirationDate is not within the expected range.");
        });
    }

    private Player player;

    private ISecureTokenFactory secureTokenFactory;

    private ISecureTokenHasher secureTokenHasher;

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<RefreshTokenRequestHandler>>();
        this.options = Substitute.For<IOptions<JwtOptions>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.secureTokenFactory = Substitute.For<ISecureTokenFactory>();
        this.secureTokenHasher = Substitute.For<ISecureTokenHasher>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.userAccountContext = Substitute.For<IUserAccountContext>();

        this.options.Value.Returns(new JwtOptions()
        {
            AccessTokenExpiryMinutes = 15,
            Audience = "audience",
            Issuer = "issuer",
            RefreshTokenExpiryDays = 1,
            Secret = "secret",
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        });

        this.player = new Player()
        {
            Name = "username",
        };

        this.userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = "username",
            EmailAddress = "test@email.com",
            Player = this.player,
        };

        this.userSessionToken = new UserSessionToken()
        {
            UserAccount = this.userAccount,
            HashedRefreshToken = "HashedRefreshToken",
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            CreatedOn = DateTime.UtcNow,
        };

        this.jwtToken = new JwtToken(
            AccessToken: "AccessToken",
            RefreshToken: "RefreshToken");

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.secureTokenFactory.GenerateJwt(Arg.Any<JwtParameters>()).Returns(this.jwtToken);
        this.secureTokenHasher.HashSecureToken(this.jwtToken.RefreshToken).Returns(this.userSessionToken.HashedRefreshToken);
        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);
        this.userAccountContext.User.Returns(this.userAccount);

        this.handler = new RefreshTokenRequestHandler(
            this.logger,
            this.options,
            this.secureTokenFactory,
            this.secureTokenHasher,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository,
            this.userAccountContext);
    }

    private IUserAccountContext userAccountContext;
}
