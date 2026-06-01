using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using Winterhaven.Gateway.Integrations.Services.Clients;

namespace Winterhaven.Gateway.Integrations.Presentation;

[TestFixture]
internal sealed class GatewayJsonRpcIntegrationTests : TestHostBase
{
    [Test]
    public async Task GatewayShouldReturnAuthorizationErrorWhenAuthenticationIsRequiredByRpc()
    {
        // Arrange
        const int expectedErrorCode = 401;
        const string expectedMessage = "Authentication is required to perform this action.";

        await using var connection = await CreateConnectionAsync(x => x
            .WithProxy<IAuthClientProxy>());

        var authProxy = connection.GetProxy<IAuthClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => authProxy.RequireAuthentication());

            Assert.That(exception.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }
    }

    [Test]
    public async Task GatewayShouldReturnAuthorizationErrorWhenAuthorizationExceptionIsThrownInRpc()
    {
        // Arrange
        const int expectedErrorCode = 401;
        const string expectedMessage = "Authentication is required to perform the request.";

        await using var connection = await CreateConnectionAsync(x => x
            .WithProxy<IErrorClientProxy>());

        var errorProxy = connection.GetProxy<IErrorClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => errorProxy.GenerateUnauthorizedError());

            Assert.That(exception.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }
    }

    [Test]
    public async Task GatewayShouldReturnDefaultValidationErrorWhenExceptionHasNoErrorsInRpc()
    {
        // Arrange
        const int expectedErrorCode = 400;
        const string expectedMessage = "One or more validation failures have occurred.";

        var expectedErrors = new Dictionary<string, string[]>()
        {
            { "General", ["One or more validation errors occurred."] }
        };

        await using var connection = await CreateConnectionAsync(x => x
            .WithProxy<IErrorClientProxy>());

        var errorProxy = connection.GetProxy<IErrorClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => errorProxy.GenerateValidationErrorWithNullErrors());

            Assert.That(exception.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));

            var actualErrors = ((JObject)exception.ErrorData).ToObject<Dictionary<string, string[]>>();

            Assert.That(actualErrors.Keys, Is.EquivalentTo(expectedErrors.Keys));
            Assert.That(actualErrors["General"], Is.EqualTo(expectedErrors["General"]));
        }
    }

    [Test]
    public async Task GatewayShouldReturnInternalErrorWhenUnhandledExceptionIsThrownInRpc()
    {
        // Arrange
        const int expectedErrorCode = (int)JsonRpcErrorCode.InternalError;
        const string expectedMessage = "An unexpected error occurred. Please try again later.";

        await using var connection = await CreateConnectionAsync(x => x
            .WithProxy<IErrorClientProxy>());

        var errorProxy = connection.GetProxy<IErrorClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => errorProxy.GenerateUnhandledError());

            Assert.That(exception.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }
    }

    [Test]
    public async Task GatewayShouldReturnResourceWhenAuthenticationIsRequiredAndConnectionIsAuthenticated()
    {
        // Arrange
        const string expectedSecret = "secret";

        await using var connection = await CreateConnectionAsync(x => x
            .WithProxy<IAuthClientProxy>());

        var authProxy = connection.GetProxy<IAuthClientProxy>();

        UserSessionContext.IsAuthenticated = true;

        // Act
        string actual = await authProxy.GetUserSecret();

        // Assert
        Assert.That(actual, Is.EqualTo(expectedSecret));
    }

    [Test]
    public async Task GatewayShouldReturnValidationErrorWhenValidationExceptionIsThrownInRpc()
    {
        // Arrange
        const int expectedErrorCode = 400;
        const string expectedMessage = "One or more validation failures have occurred.";

        var expectedErrors = new Dictionary<string, string[]>()
        {
            { "General", ["One or more validation errors occurred."] },
            { "Other", ["This is a cool test lol", "seriously cool"] }
        };

        await using var connection = await CreateConnectionAsync(x => x
            .WithProxy<IErrorClientProxy>());

        var errorProxy = connection.GetProxy<IErrorClientProxy>();

        // Act and assert
        using (Assert.EnterMultipleScope())
        {
            var exception = Assert.ThrowsAsync<RemoteInvocationException>(() => errorProxy.GenerateValidationError());

            Assert.That(exception.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));

            var actualErrors = ((JObject)exception.ErrorData).ToObject<Dictionary<string, string[]>>();

            Assert.That(actualErrors.Keys, Is.EquivalentTo(expectedErrors.Keys));
            Assert.That(actualErrors["General"], Is.EqualTo(expectedErrors["General"]));
        }
    }

    [SetUp]
    public async Task SetUp() => await SetUpTestHost().ConfigureAwait(false);

    [TearDown]
    public async Task TearDown() => await TearDownTestHost().ConfigureAwait(false);
}
