using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Services.Chat;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Presentation.Attributes;
using Winterhaven.Gateway.Presentation.Targets.Chat;

namespace Winterhaven.Gateway.Tests.Presentation.Targets.Chat;

[TestFixture]
internal sealed class ChatRpcTargetTests
{
    private IChatService chatService;

    private ILogger<ChatRpcTarget> logger;

    private ChatRpcTarget target;

    private UserSession userSession;

    private IUserSessionContext userSessionContext;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenCHatServiceIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ChatRpcTarget(this.logger, this.userSessionContext, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ChatRpcTarget(null, this.userSessionContext, this.chatService));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ChatRpcTarget(this.logger, null, this.chatService));
    }

    [Test]
    public void SendMessageAsyncShouldHaveJsonRpcAuthorizeAttribuet()
    {
        // Arrange
        var type = typeof(ChatRpcTarget);
        var property = type.GetMethod(nameof(ChatRpcTarget.SendMessageAsync), BindingFlags.Public | BindingFlags.Instance);

        // Act
        bool bound = property.GetCustomAttribute<JsonRpcAuthorizeAttribute>() is not null;

        // Assert
        Assert.That(bound, Is.True);
    }

    [Test]
    public void SendMessageAsyncShouldHaveJsonRpcMethodAttribuet()
    {
        // Arrange
        var type = typeof(ChatRpcTarget);
        var property = type.GetMethod(nameof(ChatRpcTarget.SendMessageAsync), BindingFlags.Public | BindingFlags.Instance);

        // Act
        var attribute = property.GetCustomAttribute<JsonRpcMethodAttribute>();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.Name, Is.EqualTo("chat.send_message"));
            Assert.That(attribute.UseSingleObjectParameterDeserialization, Is.True);
        }
    }

    [Test]
    public async Task SendMessageAsyncShouldInvokeChatServiceSendMessageAsyncWhenUserSessionIsValid()
    {
        // Arrange
        var parameters = new ChatSendMessageRpcParameters("test");
        var cancellationToken = new CancellationToken();

        // Act
        await this.target.SendMessageAsync(parameters, cancellationToken).ConfigureAwait(false);

        // Assert
        await this.chatService.Received(1).SendMessageAsync(parameters.Message, cancellationToken).ConfigureAwait(false);
    }

    [Test]
    public void SendMessageAsyncShouldThrowAuthorizationExceptionWhenUserSessionIsNotAuthenticated()
    {
        // Arrange
        var parameters = new ChatSendMessageRpcParameters("test");

        this.userSessionContext.IsAuthenticated.Returns(false);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.target.SendMessageAsync(parameters, CancellationToken.None));
    }

    [Test]
    public void SendMessageAsyncShouldThrowAuthorizationExceptionWhenUserSessionIsNull()
    {
        // Arrange
        var parameters = new ChatSendMessageRpcParameters("test");

        this.userSessionContext.UserSession.Returns((UserSession)null);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.target.SendMessageAsync(parameters));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<ChatRpcTarget>>();
        this.userSessionContext = Substitute.For<IUserSessionContext>();
        this.chatService = Substitute.For<IChatService>();

        this.userSession = new UserSession(
            UserAccountId: Guid.NewGuid(),
            AccessToken: "accessToken",
            ExpiresAt: DateTime.UtcNow.AddMinutes(15));

        this.userSessionContext.UserSession.Returns(this.userSession);
        this.userSessionContext.IsAuthenticated.Returns(true);

        this.target = new ChatRpcTarget(this.logger, this.userSessionContext, this.chatService);
    }

    [TearDown]
    public void TearDown()
    {
        this.userSessionContext.Dispose();
    }
}
