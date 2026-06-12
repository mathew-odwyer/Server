using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Chat;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;
using Winterhaven.Gateway.Infrastructure.Services.Chat;

namespace Winterhaven.Gateway.Tests.Infrastructure.Services.Chat;

[TestFixture]
internal sealed class ChatServiceTests
{
    private ChatService chatService;

    private ILogger<ChatService> logger;

    private IMessageBus messageBus;

    private UserSession userSession;

    private IUserSessionContext userSessionContext;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ChatService(null, this.userSessionContext, this.messageBus));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMessageBusIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ChatService(this.logger, this.userSessionContext, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserSessionContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ChatService(this.logger, null, this.messageBus));
    }

    [TestCase("/shake", "[shake]hello world")]
    [TestCase("/wobble", "[wobble]hello world")]
    [TestCase("/blink", "[blink]hello world")]
    [TestCase("/rainbow", "[rainbow]hello world")]
    public async Task SendMessageAsyncShouldConvertWhitelistEffectToScribbleFormatInBody(string effect, string expectedBody)
    {
        // Arrange
        string message = $"hello {effect} world";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.Message, Is.EqualTo(expectedBody));
    }

    [Test]
    public async Task SendMessageAsyncShouldEscapeOpenSquareBracketsInBody()
    {
        // Arrange
        const string message = "hello [player]";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.Message, Is.EqualTo("hello [[player]"));
    }

    [Test]
    public async Task SendMessageAsyncShouldLeavePartialCommandInBodyWhenTruncationSplitsACommand()
    {
        // Arrange
        string message = new string('a', 76) + " /wave";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.Message, Does.EndWith("/wa"));
    }

    [Test]
    public async Task SendMessageAsyncShouldLeaveUnknownCommandVerbatimInBody()
    {
        // Arrange
        const string message = "hello /explode world";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.Message, Is.EqualTo("hello /explode world"));
    }

    [Test]
    public async Task SendMessageAsyncShouldNormaliseExtraWhitespaceInBody()
    {
        // Arrange
        const string message = "  hello   world  ";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.Message, Is.EqualTo("hello world"));
    }

    [Test]
    public async Task SendMessageAsyncShouldProduceEmptyBodyWhenMessageContainsOnlyEmoteCommands()
    {
        // Arrange
        const string message = "/love /!";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(chatEvent.Message, Is.Empty);
            Assert.That(chatEvent.EmoteType, Is.EqualTo(ChatEmoteType.Heart));
        }
    }

    [Test]
    public async Task SendMessageAsyncShouldPublishEventWithSenderIdMatchingUserSession()
    {
        // Act
        var chatEvent = await this.GetPublishedChatEventAsync("hello").ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.SenderId, Is.EqualTo(this.userSession.UserAccountId));
    }

    [Test]
    public async Task SendMessageAsyncShouldStripEmoteAndConvertEffectWhenBothArePresent()
    {
        // Arrange
        const string message = "/love hello /shake world";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(chatEvent.Message, Is.EqualTo("[shake]hello world"));
            Assert.That(chatEvent.EmoteType, Is.EqualTo(ChatEmoteType.Heart));
        }
    }

    [TestCase("/love", ChatEmoteType.Heart)]
    [TestCase("/?", ChatEmoteType.Question)]
    [TestCase("/!", ChatEmoteType.Exclaim)]
    [TestCase("/...", ChatEmoteType.Ellipsis)]
    public async Task SendMessageAsyncShouldStripEmoteCommandFromBodyAndSetEmoteType(string command, ChatEmoteType expectedEmoteType)
    {
        // Arrange
        string message = $"hello {command} world";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(chatEvent.Message, Is.EqualTo("hello world"));
            Assert.That(chatEvent.EmoteType, Is.EqualTo(expectedEmoteType));
        }
    }

    [Test]
    public void SendMessageAsyncShouldThrowArgumentExceptionWhenMessageIsWhitespace()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => this.chatService.SendMessageAsync("   "));
    }

    [Test]
    public void SendMessageAsyncShouldThrowArgumentNullExceptionWhenMessageIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.chatService.SendMessageAsync(null));
    }

    [Test]
    public void SendMessageAsyncShouldThrowAuthorizationExceptionWhenUserIsNotAuthenticated()
    {
        // Arrange
        this.userSessionContext.IsAuthenticated.Returns(false);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.chatService.SendMessageAsync("hello"));
    }

    [Test]
    public void SendMessageAsyncShouldThrowAuthorizationExceptionWhenUserSessionIsNull()
    {
        // Arrange
        this.userSessionContext.UserSession.Returns((UserSession)null);

        // Act and assert
        Assert.ThrowsAsync<AuthorizationException>(() => this.chatService.SendMessageAsync("hello"));
    }

    [Test]
    public void SendMessageAsyncShouldThrowChatMessageExceptionWhenMessageBusExceptionIsThrownFromPublish()
    {
        // Arrange
        this.messageBus.PublishAsync(Arg.Any<ChatEvent>(), Arg.Any<PublishOptions>(), Arg.Any<CancellationToken>()).Throws(new MessageBusException());

        // Act and assert
        Assert.ThrowsAsync<ChatMessageException>(() => this.chatService.SendMessageAsync("my message", CancellationToken.None));
    }

    [Test]
    public async Task SendMessageAsyncShouldTruncateBodyTo80Characters()
    {
        // Arrange
        string message = new('a', 100);

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        Assert.That(chatEvent.Message, Has.Length.LessThanOrEqualTo(80));
    }

    [Test]
    public async Task SendMessageAsyncShouldUseLastEmoteTypeWhenMultipleEmoteCommandsAreProvided()
    {
        // Arrange
        const string message = "/love /! hello";

        // Act
        var chatEvent = await this.GetPublishedChatEventAsync(message).ConfigureAwait(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(chatEvent.Message, Is.EqualTo("hello"));
            Assert.That(chatEvent.EmoteType, Is.EqualTo(ChatEmoteType.Heart));
        }
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<ChatService>>();
        this.messageBus = Substitute.For<IMessageBus>();
        this.userSessionContext = Substitute.For<IUserSessionContext>();

        this.userSession = new UserSession(
            UserAccountId: Guid.NewGuid(),
            AccessToken: "accessToken",
            ExpiresAt: DateTime.UtcNow.AddMinutes(15));

        this.userSessionContext.IsAuthenticated.Returns(true);
        this.userSessionContext.UserSession.Returns(this.userSession);

        this.chatService = new ChatService(this.logger, this.userSessionContext, this.messageBus);
    }

    [TearDown]
    public void TearDown()
    {
        this.userSessionContext.Dispose();
    }

    private async Task<ChatEvent> GetPublishedChatEventAsync(string message, CancellationToken cancellationToken = default)
    {
        ChatEvent captured = null;

        this.messageBus
            .When(x => x.PublishAsync(Arg.Any<ChatEvent>(), Arg.Any<PublishOptions>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => captured = callInfo.ArgAt<ChatEvent>(0));

        await this.chatService.SendMessageAsync(message, cancellationToken).ConfigureAwait(false);

        return captured;
    }
}
