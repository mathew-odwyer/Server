namespace Winterhaven.API.Tests.Core.Application.Requests.Users.LoginUser;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Exceptions;
using Winterhaven.API.Core.Application.Requests.Users.LoginUser;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;

[TestFixture]
internal sealed class LoginUserRequestHandlerTests
{
    private LoginUserRequestHandler handler;

    private UserToken jwtToken;

    private ILogger<LoginUserRequestHandler> logger;

    private IUnitOfWork unitOfWork;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private UserAccount userAccount;

    private IUserAuthenticator userAuthenticator;

    private ISecureTokenFactory secureTokenFactory;

    private ISecureTokenHasher secureTokenHasher;

    private UserSessionToken userSessionToken;

    private IUserSessionTokenRepository userSessionTokenRepository;

    private Player player;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(null, this.userAuthenticator, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.userAuthenticator, this.secureTokenFactory, this.secureTokenHasher, null, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAuthenticatorIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, null, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.userAuthenticator, null, this.secureTokenHasher, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenHasherIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.userAuthenticator, this.secureTokenFactory, null, this.unitOfWorkFactory, this.userSessionTokenRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesionTokenRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(this.logger, this.userAuthenticator, this.secureTokenFactory, this.secureTokenHasher, this.unitOfWorkFactory, null));
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
            Username: this.userAccount.Username,
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
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.secureTokenHasher.Received(1).HashSecureToken("RefreshToken");
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenWhenActionSessionIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.secureTokenFactory.Received(1).GenerateUserToken(Arg.Any<UserTokenParameters>());
    }

    [Test]
    public void HandleShouldThrowAuthorizationExceptionWhenGetActiveSessionAsyncReturnsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.Username,
            Password: "password");

        this.userSessionTokenRepository.GetActiveSessionAsync(this.userAccount.Id, default).Returns(this.userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.Username,
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
            Username: this.userAccount.Username,
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
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        this.userAuthenticator.Received(1).LoginUserAsync(request.Username, request.Password);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldReturnJwtAccessAndRefreshTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.AccessToken, Is.EqualTo(this.jwtToken.AccessToken));
            Assert.That(response.RefreshToken, Is.EqualTo(this.jwtToken.RefreshToken));
        }
    }

    [Test]
    public void HandleShouldThrowAuthorizationExceptionWhenSaveAsyncThrowsDatabaseUpdateException()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.Username,
            Password: "password");

        this.unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: this.userAccount.Username,
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
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.userSessionTokenRepository.Received(1).AddAsync(
            Arg.Is<UserSessionToken>(token =>
                token.UserAccount == this.userAccount &&
                token.HashedRefreshToken == this.userSessionToken.HashedRefreshToken &&
                token.AccessTokenExpirationDate > DateTime.UtcNow &&
                token.IsRevoked == this.userSessionToken.IsRevoked &&
                token.AccessTokenExpirationDate <= DateTime.UtcNow.AddMinutes(15)
            ),
            Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<LoginUserRequestHandler>>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();
        this.userAuthenticator = Substitute.For<IUserAuthenticator>();
        this.secureTokenFactory = Substitute.For<ISecureTokenFactory>();
        this.secureTokenHasher = Substitute.For<ISecureTokenHasher>();
        this.userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();

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

        this.jwtToken = new UserToken(
            AccessToken: "AccessToken",
            RefreshToken: "RefreshToken",
            AccessTokenExpiryDate: DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiryDate: DateTime.UtcNow.AddDays(7));

        this.userSessionToken = new UserSessionToken()
        {
            UserAccount = this.userAccount,
            HashedRefreshToken = "HashedRefreshToken",
            AccessTokenExpirationDate = this.jwtToken.AccessTokenExpiryDate,
            RefreshTokenExpirationDate = this.jwtToken.RefreshTokenExpiryDate,
        };

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);

        this.userAuthenticator.LoginUserAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(this.userAccount);
        this.secureTokenFactory.GenerateUserToken(Arg.Any<UserTokenParameters>()).Returns(this.jwtToken);
        this.secureTokenHasher.HashSecureToken(this.jwtToken.RefreshToken).Returns(this.userSessionToken.HashedRefreshToken);

        this.handler = new LoginUserRequestHandler(
            this.logger,
            this.userAuthenticator,
            this.secureTokenFactory,
            this.secureTokenHasher,
            this.unitOfWorkFactory,
            this.userSessionTokenRepository);
    }
}