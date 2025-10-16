// <copyright file="LoginUserRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.LoginUser;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Options.Security;
using Web.Application.Requests.Users.LoginUser;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class LoginUserRequestHandlerTests
{
    private LoginUserRequestHandler handler;

    private JwtToken jwtToken;

    private ILogger<LoginUserRequestHandler> logger;

    private IOptions<JwtOptions> options;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private IUserAccountService userAccountService;

    private IUserAccountTokenService userAccountTokenService;

    private UserSessionToken userSessionToken;

    private IUserSessionTokenRepository userSessionTokenRepository;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(null, this.options, this.userAccountService, this.userAccountTokenService, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, null, this.userAccountService, this.userAccountTokenService, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.options, this.userAccountService, this.userAccountTokenService, null, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountServiceIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.options, null, this.userAccountTokenService, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountTokenServiceIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.options, this.userAccountService, null, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.options, this.userAccountService, this.userAccountTokenService, this.unitOfWorkFactory, null));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public async Task HandleShouldInvokeUserSessionTokenRepositoryAddAsyncWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).AddAsync(Arg.Any<UserSessionToken>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeHashRefreshTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.userAccountTokenService.Received(1).HashSecureToken("RefreshToken");
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenWhenActionSessionIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.userAccountTokenService.Received(1).GenerateJwt(Arg.Any<JwtParameters>());
    }

    [Test]
    public void HandleShouldThrowUnauthorizedExceptionWhenGetActiveSessionAsyncReturnsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<UnauthorizedException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).GetActiveSessionAsync(this.userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public async Task HandleShouldInvokeLoginUserAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userAccountService.Received(1).LoginUserAsync(request.Username, request.Password);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldReturnJwtAccessAndRefreshTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

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
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        this.unitOfWork.SaveAsync(default).ThrowsAsync<DatabaseUpdateConcurrencyException>();

        // Act and assert
        Assert.ThrowsAsync<UnauthorizedException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldAddCorrectUserSessionTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.UserName,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userSessionTokenRepository.Received(1).AddAsync(
            Arg.Is<UserSessionToken>(token =>
                token.UserAccount == this.userAccount &&
                token.HashedRefreshToken == this.userSessionToken.HashedRefreshToken &&
                token.SessionId == this.jwtToken.SessionId &&
                token.ExpirationDate > DateTime.UtcNow &&
                token.ExpirationDate <= DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes)
            ),
            Arg.Any<CancellationToken>());

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<LoginUserRequestHandler>>();
        this.options = Substitute.For<IOptions<JwtOptions>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.userAccountService = Substitute.For<IUserAccountService>();
        this.userAccountTokenService = Substitute.For<IUserAccountTokenService>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();

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
        };

        this.jwtToken = new JwtToken(
            AccessToken: "AccessToken",
            RefreshToken: "RefreshToken",
            SessionId: this.userSessionToken.SessionId);

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.userAccountService.LoginUserAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(this.userAccount);
        this.userAccountTokenService.GenerateJwt(Arg.Any<JwtParameters>()).Returns(this.jwtToken);
        this.userAccountTokenService.HashSecureToken(this.jwtToken.RefreshToken).Returns(this.userSessionToken.HashedRefreshToken);

        this.handler = new LoginUserRequestHandler(
            this.logger,
            this.options,
            this.userAccountService,
            this.userAccountTokenService,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository);
    }
}
