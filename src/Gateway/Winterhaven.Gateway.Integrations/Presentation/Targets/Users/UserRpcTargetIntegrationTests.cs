using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Integrations.Services.Clients;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Winterhaven.Gateway.Integrations.Presentation.Targets.Users;

[TestFixture]
internal sealed class UserRpcTargetIntegrationTests : TestHostBase
{
    [SetUp]
    public async Task SetUp()
    {
        await SetUpTestHost();
        UserSessionManager.InvalidateUserSession();
    }

    [TearDown]
    public async Task TearDown() => await TearDownTestHost();

    [Test]
    public async Task UserLoginRequestShouldLoginUserWhenCredentialsAreCorrect()
    {
        // Arrange
        const string username = "testuser";
        const string password = "password";

        var apiResponse = new LoginUserResponseDto(
            AccessToken: CreateAccessToken(Guid.NewGuid(), username),
            RefreshToken: "refreshToken");

        Api
            .Given(Request.Create().WithPath("/api/UserAccount/Login").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(apiResponse));

        await using var connection = await CreateConnectionAsync(
           x => x.WithProxy<IUserClientProxy>());

        var userProxy = connection.GetProxy<IUserClientProxy>();

        // Act
        var response = await userProxy.LoginAsync(
            username: username,
            password: password);

        // Assert
        Assert.That(response.RefreshToken, Is.EqualTo(apiResponse.RefreshToken));
    }

    [Test]
    public async Task UserLoginRequestShouldReturnAuthorizationErrorWhenUserAlreadyLoggedIn()
    {
        // Arrange
        const string username = "testuser";
        const string password = "password";

        var apiResponse = new
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = 401,
            TraceId = Guid.NewGuid().ToString(),
        };

        Api
            .Given(Request.Create().WithPath("/api/UserAccount/Login").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Unauthorized).WithBodyAsJson(apiResponse));

        var identifier = Guid.NewGuid();

        UserSessionManager.EstablishUserSession(new UserSession(
            UserAccountId: identifier,
            Username: username,
            AccessToken: CreateAccessToken(identifier, username),
            ExpiresAt: DateTime.UtcNow.AddMinutes(15)));

        await using var connection = await CreateConnectionAsync(
            x => x.WithProxy<IUserClientProxy>());

        var userProxy = connection.GetProxy<IUserClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => userProxy.LoginAsync(username, password));

            Assert.That(exception.ErrorCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(exception.Message, Is.EqualTo("An active session already exists for this connection."));
        }
    }

    [Test]
    public async Task UserRefreshShouldReturnAuthorizationErrorWhenUserIsNotLoggedIn()
    {
        var apiResponse = new
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = 401,
            TraceId = Guid.NewGuid().ToString(),
        };

        Api
            .Given(Request.Create().WithPath("/api/UserAccount/RefreshToken").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Unauthorized).WithBodyAsJson(apiResponse));

        await using var connection = await CreateConnectionAsync(
            x => x.WithProxy<IUserClientProxy>());

        var userProxy = connection.GetProxy<IUserClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => userProxy.RefreshAsync("refreshToken"));

            Assert.That(exception.ErrorCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(exception.Message, Is.EqualTo("Authentication is required to perform this action."));
        }
    }

    [Test]
    public async Task UserRegisterRequestShouldRegisterUserWhenCredentialsMatchRequirements()
    {
        // Arrange
        const string username = "testuser";
        const string password = "password";
        const string emailAddress = "test@email.com";

        Api
            .Given(Request.Create().WithPath("/api/UserAccount/Register").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.NoContent));

        await using var connection = await CreateConnectionAsync(
            x => x.WithProxy<IUserClientProxy>());

        var userProxy = connection.GetProxy<IUserClientProxy>();

        // Act
        var response = await userProxy.RegisterAsync(
            username: username,
            password: password,
            email_address: emailAddress);

        // Assert
        Assert.That(response, Is.Not.Null);
    }
}
