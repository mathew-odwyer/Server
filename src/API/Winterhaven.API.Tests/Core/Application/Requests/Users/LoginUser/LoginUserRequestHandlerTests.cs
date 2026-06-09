using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Users.LoginUser;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Core.Domain.ValueObjects.Users;
using Winterhaven.Common.Exceptions;

namespace Winterhaven.API.Tests.Core.Application.Requests.Users.LoginUser;

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
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(null, userAuthenticator, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(logger, userAuthenticator, secureTokenFactory, secureTokenHasher, null, userSessionTokenRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAuthenticatorIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(logger, null, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(logger, userAuthenticator, null, secureTokenHasher, unitOfWorkFactory, userSessionTokenRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenSecureTokenHasherIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(logger, userAuthenticator, secureTokenFactory, null, unitOfWorkFactory, userSessionTokenRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSesionTokenRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new LoginUserRequestHandler(logger, userAuthenticator, secureTokenFactory, secureTokenHasher, unitOfWorkFactory, null));

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, default));

    [Test]
    public async Task HandleShouldInvokeUserSessionTokenRepositoryAddAsyncWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userSessionTokenRepository.Received(1).AddAsync(Arg.Any<UserSessionToken>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeHashRefreshTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        secureTokenHasher.Received(1).HashSecureToken("RefreshToken");
    }

    [Test]
    public async Task HandleShouldInvokeGenerateJwtWhenWhenActionSessionIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        secureTokenFactory.Received(1).GenerateUserToken(Arg.Any<UserTokenParameters>());
    }

    [Test]
    public void HandleShouldThrowAuthorizationExceptionWhenGetActiveSessionAsyncReturnsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, default).Returns(userSessionToken);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeGetActiveSessionAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userSessionTokenRepository.Received(1).GetActiveSessionAsync(userAccount.Id, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public async Task HandleShouldInvokeLoginUserAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        userAuthenticator.Received(1).AuthenticateUser(request.Username, request.Password);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldReturnJwtAccessAndRefreshTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        var response = await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.AccessToken, Is.EqualTo(jwtToken.AccessToken));
            Assert.That(response.RefreshToken, Is.EqualTo(jwtToken.RefreshToken));
        }
    }

    [Test]
    public void HandleShouldThrowAuthorizationExceptionWhenSaveAsyncThrowsDatabaseUpdateException()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        unitOfWork.SaveAsync(default).ThrowsAsync<EntityPersistenceException>();

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(request, default));
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldAddCorrectUserSessionTokenWhenActiveSessionIsNull()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await userSessionTokenRepository.Received(1).AddAsync(
            Arg.Is<UserSessionToken>(token =>
                token.UserAccount == userAccount &&
                token.HashedRefreshToken == userSessionToken.HashedRefreshToken &&
                token.AccessTokenExpirationDate > DateTime.UtcNow &&
                token.IsRevoked == userSessionToken.IsRevoked &&
                token.AccessTokenExpirationDate <= DateTime.UtcNow.AddMinutes(15)
            ),
            Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<LoginUserRequestHandler>>();
        unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        userAuthenticator = Substitute.For<IUserAuthenticator>();
        secureTokenFactory = Substitute.For<ISecureTokenFactory>();
        secureTokenHasher = Substitute.For<ISecureTokenHasher>();
        userSessionTokenRepository = Substitute.For<IUserSessionTokenRepository>();

        player = new Player()
        {
            Name = "username",
        };

        userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = "username",
            EmailAddress = "test@email.com",
            Player = player,
        };

        jwtToken = new UserToken(
            AccessToken: "AccessToken",
            RefreshToken: "RefreshToken",
            AccessTokenExpiryDate: DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiryDate: DateTime.UtcNow.AddDays(7));

        userSessionToken = new UserSessionToken()
        {
            UserAccount = userAccount,
            HashedRefreshToken = "HashedRefreshToken",
            AccessTokenExpirationDate = jwtToken.AccessTokenExpiryDate,
            RefreshTokenExpirationDate = jwtToken.RefreshTokenExpiryDate,
        };

        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);

        userAuthenticator.AuthenticateUser(Arg.Any<string>(), Arg.Any<string>()).Returns(userAccount);
        secureTokenFactory.GenerateUserToken(Arg.Any<UserTokenParameters>()).Returns(jwtToken);
        secureTokenHasher.HashSecureToken(jwtToken.RefreshToken).Returns(userSessionToken.HashedRefreshToken);

        handler = new LoginUserRequestHandler(
            logger,
            userAuthenticator,
            secureTokenFactory,
            secureTokenHasher,
            unitOfWorkFactory,
            userSessionTokenRepository);
    }
}
