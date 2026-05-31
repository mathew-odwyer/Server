using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Pipeline.Handlers;

namespace Winterhaven.Gateway.Tests.Infrastructure.Pipeline.Handlers;

[TestFixture]
internal sealed class AccessTokenHandlerTests
{
    private HttpClient client;

    private AccessTokenHandler handler;

    private HttpContext httpContext;

    private IHttpContextAccessor httpContextAccessor;

    private IServiceProvider serviceProvider;

    private IUserSessionContext userSessionContext;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenHttpContextAccessorIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new AccessTokenHandler(null));

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task SendAsyncShouldNotSetAuthorizationHeaderWhenAccessTokenIsInvalid(string token)
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "user", token, DateTimeOffset.UtcNow.AddSeconds(5));
        userSessionContext.UserSession.Returns(session);
        serviceProvider.GetService(typeof(IUserSessionContext)).Returns(userSessionContext);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        // Assert
        Assert.That(request.Headers.Authorization, Is.Null);
    }

    [Test]
    public async Task SendAsyncShouldNotSetAuthorizationHeaderWhenHttpContextIsNull()
    {
        // Arrange
        httpContextAccessor.HttpContext.Returns((HttpContext)null);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        // Assert
        Assert.That(request.Headers.Authorization, Is.Null);
    }

    [Test]
    public async Task SendAsyncShouldNotSetAuthorizationHeaderWhenRequestServicesIsNull()
    {
        // Arrange
        httpContext.RequestServices.Returns((IServiceProvider)null);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        // Assert
        Assert.That(request.Headers.Authorization, Is.Null);
    }

    [Test]
    public async Task SendAsyncShouldNotSetAuthorizationHeaderWhenUserSessionContextIsMissing()
    {
        // Arrange
        serviceProvider.GetService(typeof(IUserSessionContext)).Returns(null);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        // Assert
        Assert.That(request.Headers.Authorization, Is.Null);
    }

    [Test]
    public async Task SendAsyncShouldNotSetAuthorizationHeaderWhenUserSessionIsNull()
    {
        // Arrange
        userSessionContext.UserSession.Returns((UserSession)null);
        serviceProvider.GetService(typeof(IUserSessionContext)).Returns(userSessionContext);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        // Assert
        Assert.That(request.Headers.Authorization, Is.Null);
    }

    [Test]
    public async Task SendAsyncShouldOverwriteExistingAuthorizationHeaderWhenValidTokenExists()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "user", "new-token", DateTimeOffset.UtcNow.AddSeconds(5));
        userSessionContext.UserSession.Returns(session);
        serviceProvider.GetService(typeof(IUserSessionContext)).Returns(userSessionContext);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "old-token");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        // Assert
        Assert.That(request.Headers.Authorization.Parameter, Is.EqualTo("new-token"));
    }

    [Test]
    public async Task SendAsyncShouldSetBearerAuthorizationHeaderWhenAccessTokenIsValid()
    {
        // Arrange
        var session = new UserSession(Guid.NewGuid(), "user", "valid-token", DateTimeOffset.UtcNow.AddSeconds(5));
        userSessionContext.UserSession.Returns(session);
        serviceProvider.GetService(typeof(IUserSessionContext)).Returns(userSessionContext);

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://test");

        // Act
        await client.SendAsync(request).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(request.Headers.Authorization.Scheme, Is.EqualTo("Bearer"));
            Assert.That(request.Headers.Authorization.Parameter, Is.EqualTo("valid-token"));
        }
    }

    [SetUp]
    public void SetUp()
    {
        // Arrange
        httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContext = Substitute.For<HttpContext>();
        serviceProvider = Substitute.For<IServiceProvider>();
        userSessionContext = Substitute.For<IUserSessionContext>();

        httpContextAccessor.HttpContext.Returns(httpContext);
        httpContext.RequestServices.Returns(serviceProvider);

        handler = new AccessTokenHandler(httpContextAccessor)
        {
            InnerHandler = new TestHttpMessageHandler()
        };

        client = new HttpClient(handler);
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
        handler.Dispose();
        userSessionContext.Dispose();
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}
