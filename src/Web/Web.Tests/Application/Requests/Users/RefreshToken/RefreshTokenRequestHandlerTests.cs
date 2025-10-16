// <copyright file="RefreshTokenRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.RefreshToken;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Options.Security;
using Web.Application.Requests.Users.RefreshToken;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;

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

    private IUserAccountTokenService userAccountTokenService;

    private UserSessionToken userSessionToken;

    private IUserSessionTokenRepository userSessionTokenRepository;

    [Test]
    public async Task HandleShouldInvokeHashRefreshTokenWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.userAccountTokenService.Received(2).HashSecureToken(request.RefreshToken);
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenUserAccountIsFound()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.userAccountTokenService.Received(1).GenerateJwt(Arg.Any<JwtParameters>());
    }

    [Test]
    public async Task HandleShouldSetExpirationDateToUtcNowWhenGetActiveSessionAsyncDoesNotReturnNull()
    {
        // Arrange
        var expected = DateTime.UtcNow;

        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(this.userSessionToken.ExpirationDate, Is.InRange(expected.AddSeconds(-1), expected.AddSeconds(1)));
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
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
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = this.userSessionToken.HashedRefreshToken,
            SessionId = this.userSessionToken.SessionId,
            UserAccount = this.userAccount,
            CreatedOn = DateTime.UtcNow.AddDays(-100),
        };

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<UnauthorizedException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsIncorrectHashedRefreshToken()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        var userSessionToken = new UserSessionToken()
        {
            HashedRefreshToken = "Test",
            SessionId = this.userSessionToken.SessionId,
            UserAccount = this.userAccount,
        };

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<UnauthorizedException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<UnauthorizedException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
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
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(null, this.options, this.userAccountTokenService, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, null, this.userAccountTokenService, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, this.userAccountTokenService, null, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountTokenServiceIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, null, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RefreshTokenRequestHandler(this.logger, this.options, this.userAccountTokenService, this.unitOfWorkFactory, null));
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
            UserAccountId: this.userAccount.Id,
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
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateConcurrencyException>();

        // Act and assert
        Assert.ThrowsAsync<UnauthorizedException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenNewSessionHasBeenCreated()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: this.userAccount.Id,
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
            UserAccountId: this.userAccount.Id,
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
            UserAccountId: this.userAccount.Id,
            RefreshToken: this.jwtToken.RefreshToken);

        UserSessionToken addedSession = null;

        this.userSessionTokenRepository
            .When(x => x.AddAsync(Arg.Any<UserSessionToken>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => addedSession = callInfo.Arg<UserSessionToken>());

        string expectedHashedRefreshToken = this.userSessionToken.HashedRefreshToken;
        var expectedSessionId = this.jwtToken.SessionId;
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
            Assert.That(addedSession.SessionId, Is.EqualTo(expectedSessionId), "SessionId does not match.");
            Assert.That(
                addedSession.ExpirationDate,
                Is.InRange(utcNow.AddMinutes(expectedExpiryMinutes - 1), utcNow.AddMinutes(expectedExpiryMinutes + 1)),
                "ExpirationDate is not within the expected range.");
        });
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<RefreshTokenRequestHandler>>();
        this.options = Substitute.For<IOptions<JwtOptions>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.userAccountTokenService = Substitute.For<IUserAccountTokenService>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

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

        this.userAccount = new UserAccount()
        {
            Id = "0",
            UserName = "username",
        };

        this.userSessionToken = new UserSessionToken()
        {
            UserAccount = this.userAccount,
            HashedRefreshToken = "HashedRefreshToken",
            SessionId = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            CreatedOn = DateTime.UtcNow,
        };

        this.jwtToken = new JwtToken(
            AccessToken: "AccessToken",
            RefreshToken: "RefreshToken",
            SessionId: this.userSessionToken.SessionId);

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.userAccountTokenService.GenerateJwt(Arg.Any<JwtParameters>()).Returns(this.jwtToken);
        this.userAccountTokenService.HashSecureToken(this.jwtToken.RefreshToken).Returns(this.userSessionToken.HashedRefreshToken);
        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);

        this.handler = new RefreshTokenRequestHandler(
            this.logger,
            this.options,
            this.userAccountTokenService,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository);
    }
}
