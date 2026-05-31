using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Tests.Infrastructure.Services.Users;

[TestFixture]
internal sealed class UserAccountServiceTests
{
    private ILogger<UserAccountService> logger;

    private IUserAccountClient userAccountClient;

    private UserAccountService userAccountService;

    private UserSession userSession;

    private IUserSessionContext userSessionContext;

    private IUserSessionManager userSessionManager;

    private IUserTokenParser userTokenParser;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(null, userAccountClient, userSessionManager, userSessionContext, userTokenParser));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountClientIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(logger, null, userSessionManager, userSessionContext, userTokenParser));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(logger, userAccountClient, userSessionManager, null, userTokenParser));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenuserSessionManagerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(logger, userAccountClient, null, userSessionContext, userTokenParser));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserTokenParserIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(logger, userAccountClient, userSessionManager, userSessionContext, null));

    [Test]
    public async Task LoginAsyncShouldCancelWhenUserAccountClientLoginAsyncCancels()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";

        userAccountClient.LoginUserAsync(
            Arg.Is<LoginUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(new LoginUserResponseDto(
                AccessToken: "accessToken",
                RefreshToken: "refreshToken")));

        // Act
        await userAccountService.LoginAsync(username, password, new CancellationToken(true)).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).LoginUserAsync(
            Arg.Is<LoginUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password),
            Arg.Is<CancellationToken>(ct => ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public async Task LoginAsyncShouldInvokeUserAccountServiceLoginUserAsyncWhenParametersAreNotNullEmptyOrWhiteSpace()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";

        userAccountClient.LoginUserAsync(
            Arg.Is<LoginUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(new LoginUserResponseDto(
                AccessToken: "accessToken",
                RefreshToken: "refreshToken")));

        // Act
        await userAccountService.LoginAsync(username, password).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).LoginUserAsync(
            Arg.Is<LoginUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password),
            Arg.Is<CancellationToken>(ct => !ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public async Task LoginAsyncShouldInvokeuserSessionManagerAuthenticateWhenUserIsNotAuthenticated()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(false);

        // Act
        await userAccountService.LoginAsync(userSession.Username, "password").ConfigureAwait(false);

        // Assert
        userSessionManager.Received(1).EstablishUserSession(
            Arg.Is<UserSession>(x =>
                x.Username == userSession.Username &&
                x.UserAccountId == userSession.UserAccountId &&
                x.AccessToken == userSession.AccessToken &&
                x.ExpiresAt == userSession.ExpiresAt));
    }

    [Test]
    public async Task LoginAsyncShouldInvokeUserTokenParseParseUserTokenWhenUserIsNotAuthenticated()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(false);

        // Act
        await userAccountService.LoginAsync(userSession.Username, "password").ConfigureAwait(false);

        // Assert
        userTokenParser.Received(1).ParseUserToken("accessToken");
        Assert.Pass();
    }

    [Test]
    public async Task LoginAsyncShouldReturnRefreshTokenWhenParametersAreNotNullEmptyOrWhiteSpace()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";

        userAccountClient.LoginUserAsync(Arg.Is<LoginUserRequestDto>(
            dto =>
                dto.Username == username &&
                dto.Password == password),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(new LoginUserResponseDto(
                AccessToken: "accessToken",
                RefreshToken: "refreshToken")));

        // Act
        var response = await userAccountService.LoginAsync(username, password).ConfigureAwait(false);

        // Assert
        Assert.That(response.RefreshToken, Is.EqualTo("refreshToken"));
    }

    [Test]
    public void LoginAsyncShouldThrowArgumentExceptionWhenPasswordIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.LoginAsync("username", string.Empty));

    [Test]
    public void LoginAsyncShouldThrowArgumentExceptionWhenPasswordIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.LoginAsync("username", "\r\t\n "));

    [Test]
    public void LoginAsyncShouldThrowArgumentExceptionWhenUsernameIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.LoginAsync(string.Empty, "password"));

    [Test]
    public void LoginAsyncShouldThrowArgumentExceptionWhenUsernameIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.LoginAsync("\r\t\n ", "password"));

    [Test]
    public void LoginAsyncShouldThrowArgumentNullExceptionWhenPasswordIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.LoginAsync("username", null));

    [Test]
    public void LoginAsyncShouldThrowArgumentNullExceptionWhenUsernameIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.LoginAsync(null, "password"));

    [Test]
    public void LoginAsyncShouldThrowAuthenticationExceptionWhenuserSessionManagerIsAuthenticatedIsTrue()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => userAccountService.LoginAsync("username", "password"));
    }

    [Test]
    public async Task LogoutAsyncShouldInvalidateUserSessionWhenUserIsAuthenticated()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        // Act
        await userAccountService.LogoutAsync().ConfigureAwait(false);

        // Assert
        userSessionManager.Received(1).InvalidateUserSession();
    }

    [Test]
    public async Task LogoutAsyncShouldInvokeUserAccountClientLogoutUserAsyncWhenUserIsAuthenticated()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        // Act
        await userAccountService.LogoutAsync().ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).LogoutUserAsync(Arg.Is<CancellationToken>(ct => !ct.IsCancellationRequested)).ConfigureAwait(false);
    }

    [Test]
    public async Task LogoutAsyncShouldPassCancellationTokenToUserAccountClient()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        // Act
        await userAccountService.LogoutAsync(cts.Token).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1)
            .LogoutUserAsync(Arg.Is<CancellationToken>(ct => ct.IsCancellationRequested))
            .ConfigureAwait(false);
    }

    [Test]
    public async Task LogoutAsyncShouldReturnImmediatelyWhenUserIsNotAuthenticated()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(false);

        // Act
        await userAccountService.LogoutAsync().ConfigureAwait(false);

        // Assert
        await userAccountClient.DidNotReceive().LogoutUserAsync(Arg.Any<CancellationToken>()).ConfigureAwait(false);
        userSessionManager.DidNotReceive().InvalidateUserSession();
    }

    [Test]
    public async Task LogoutAsyncShouldReturnImmediatelyWhenUserSessionIsNull()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);
        userSessionContext.UserSession.Returns((UserSession)null);

        // Act
        await userAccountService.LogoutAsync().ConfigureAwait(false);

        // Assert
        await userAccountClient.DidNotReceive().LogoutUserAsync(Arg.Any<CancellationToken>()).ConfigureAwait(false);
        userSessionManager.DidNotReceive().InvalidateUserSession();
    }

    [Test]
    public async Task RefreshTokenAsyncShouldCancelWhenUserAccountClientRefreshTokenAsyncCancels()
    {
        // Arrange
        const string refreshToken = "myRefreshToken";
        userSessionContext.IsAuthenticated.Returns(true);

        // Act
        await userAccountService
            .RefreshTokenAsync(refreshToken, new CancellationToken(true))
            .ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).RefreshTokenAsync(
            Arg.Is<RefreshTokenRequestDto>(dto => dto.RefreshToken == refreshToken),
            Arg.Is<CancellationToken>(ct => ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public async Task RefreshTokenAsyncShouldInvokeUserAccountClientRefreshTokenAsyncWhenParametersAreValid()
    {
        // Arrange
        const string refreshToken = "myRefreshToken";
        userSessionContext.IsAuthenticated.Returns(true);

        // Act
        await userAccountService.RefreshTokenAsync(refreshToken).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).RefreshTokenAsync(
            Arg.Is<RefreshTokenRequestDto>(dto => dto.RefreshToken == refreshToken),
            Arg.Is<CancellationToken>(ct => !ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public async Task RefreshTokenAsyncShouldInvokeUserSessionManagerRefreshUserSessionWithParsedSession()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        var newSession = new UserSession(
            UserAccountId: Guid.NewGuid(),
            Username: "cooluser",
            AccessToken: "newAccessToken",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15));

        userAccountClient.RefreshTokenAsync(
            Arg.Any<RefreshTokenRequestDto>(),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(new RefreshTokenResponseDto(
                AccessToken: "newAccessToken",
                RefreshToken: "newRefreshToken")));

        userTokenParser.ParseUserToken("newAccessToken").Returns(newSession);

        // Act
        await userAccountService.RefreshTokenAsync("refreshToken").ConfigureAwait(false);

        // Assert
        userSessionManager.Received(1).RefreshUserSession(
            Arg.Is<UserSession>(x =>
                x.Username == newSession.Username &&
                x.UserAccountId == newSession.UserAccountId &&
                x.AccessToken == newSession.AccessToken &&
                x.ExpiresAt == newSession.ExpiresAt));
    }

    [Test]
    public async Task RefreshTokenAsyncShouldInvokeUserTokenParserParseUserTokenWithAccessTokenFromResponse()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        userAccountClient.RefreshTokenAsync(
            Arg.Any<RefreshTokenRequestDto>(),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(new RefreshTokenResponseDto(
                AccessToken: "newAccessToken",
                RefreshToken: "newRefreshToken")));

        // Act
        await userAccountService.RefreshTokenAsync("refreshToken").ConfigureAwait(false);

        // Assert
        userTokenParser.Received(1).ParseUserToken("newAccessToken");
    }

    [Test]
    public async Task RefreshTokenAsyncShouldReturnRefreshTokenFromResponse()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);

        userAccountClient.RefreshTokenAsync(
            Arg.Any<RefreshTokenRequestDto>(),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(new RefreshTokenResponseDto(
                AccessToken: "newAccessToken",
                RefreshToken: "newRefreshToken")));

        // Act
        var response = await userAccountService.RefreshTokenAsync("refreshToken").ConfigureAwait(false);

        // Assert
        Assert.That(response.RefreshToken, Is.EqualTo("newRefreshToken"));
    }

    [Test]
    public void RefreshTokenAsyncShouldThrowArgumentExceptionWhenRefreshTokenIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RefreshTokenAsync(string.Empty));

    [Test]
    public void RefreshTokenAsyncShouldThrowArgumentExceptionWhenRefreshTokenIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RefreshTokenAsync("\r\t\n "));

    [Test]
    public void RefreshTokenAsyncShouldThrowArgumentNullExceptionWhenRefreshTokenIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RefreshTokenAsync(null));

    [Test]
    public void RefreshTokenAsyncShouldThrowAuthorizationExceptionWhenUserIsNotAuthenticated()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(false);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => userAccountService.RefreshTokenAsync("refreshToken"));
    }

    [Test]
    public void RefreshTokenAsyncShouldThrowAuthorizationExceptionWhenUserSessionIsNull()
    {
        // Arrange
        userSessionContext.IsAuthenticated.Returns(true);
        userSessionContext.UserSession.Returns((UserSession)null);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => userAccountService.RefreshTokenAsync("refreshToken"));
    }

    [Test]
    public async Task RegisterAsyncShouldCancelWhenUserAccountClientRegusterAsyncCancels()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";
        const string emailAddress = "user@email.com";

        // Act
        await userAccountService.RegisterAsync(username, password, emailAddress, new CancellationToken(true)).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).RegisterUserAsync(
            Arg.Is<RegisterUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password &&
                dto.EmailAddress == emailAddress),
            Arg.Is<CancellationToken>(ct => ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public async Task RegisterAsyncShouldInvokeUserAccountServiceRegisterUserAsyncWhenParametersAreNotNullEmptyOrWhiteSpace()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";
        const string emailAddress = "user@email.com";

        // Act
        await userAccountService.RegisterAsync(username, password, emailAddress).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).RegisterUserAsync(
            Arg.Is<RegisterUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password &&
                dto.EmailAddress == emailAddress),
            Arg.Is<CancellationToken>(ct => !ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenEmailAddressIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", "password", string.Empty));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenEmailAddressIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", "password", "\r\t\n "));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenPasswordIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", string.Empty, "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenPasswordIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", "\r\t\n ", "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenUsernameIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync(string.Empty, "password", "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenUsernameIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("\r\t\n ", "password", "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentNullExceptionWhenEmailAddressIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RegisterAsync("username", "password", null));

    [Test]
    public void RegisterAsyncShouldThrowArgumentNullExceptionWhenPasswordIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RegisterAsync("username", null, "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentNullExceptionWhenUsernameIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RegisterAsync(null, "password", "email"));

    [SetUp]
    public void SetUp()
    {
        // Arrange
        logger = Substitute.For<ILogger<UserAccountService>>();
        userAccountClient = Substitute.For<IUserAccountClient>();
        userSessionManager = Substitute.For<IUserSessionManager>();
        userSessionContext = Substitute.For<IUserSessionContext>();
        userTokenParser = Substitute.For<IUserTokenParser>();

        userAccountClient.LoginUserAsync(Arg.Any<LoginUserRequestDto>()).Returns(Task.FromResult(new LoginUserResponseDto(
            AccessToken: "accessToken",
            RefreshToken: "refreshToken")));

        userAccountClient.RefreshTokenAsync(Arg.Any<RefreshTokenRequestDto>()).Returns(Task.FromResult(new RefreshTokenResponseDto(
            AccessToken: "accessToken2",
            RefreshToken: "refreshToken2")));

        userSession = new UserSession(
            UserAccountId: Guid.NewGuid(),
            Username: "cooluser",
            AccessToken: "accessToken",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15));

        userTokenParser.ParseUserToken("accessToken").Returns(userSession);

        userSessionContext.IsAuthenticated.Returns(false);
        userSessionContext.UserSession.Returns(userSession);

        userAccountService = new UserAccountService(logger, userAccountClient, userSessionManager, userSessionContext, userTokenParser);
    }

    [TearDown]
    public void TearDown() => userSessionContext.Dispose();
}
