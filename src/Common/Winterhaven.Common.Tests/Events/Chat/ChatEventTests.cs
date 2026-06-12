using System;
using NUnit.Framework;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Chat;

namespace Winterhaven.Common.Tests.Events.Chat;

[TestFixture]
internal sealed class ChatEventTests
{
    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new ChatEvent()
        {
            Message = "Test message",
        });
    }

    [Test]
    public void GetPublishEventRouteShouldReturnEventRoute()
    {
        // Arrange
        const string expected = "chat.message";

        // Act
        string actual = ChatEvent.GetPublishEventRoute(new PublishOptions());

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetPublishEventRouteShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => ChatEvent.GetPublishEventRoute(null));
    }

    [Test]
    public void GetShouldReturnExpectedWhenInvoked()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        const string message = "this is a message lol.";
        const ChatEmoteType emoteType = ChatEmoteType.Ellipsis;

        // Act
        var notification = new ChatEvent()
        {
            SenderId = senderId,
            Message = message,
            EmoteType = emoteType,
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(notification.SenderId, Is.EqualTo(senderId));
            Assert.That(notification.Message, Is.EqualTo(message));
            Assert.That(notification.EmoteType, Is.EqualTo(emoteType));
        }
    }

    [Test]
    public void GetSubscribeEventRouteShouldReturnEventRoute()
    {
        // Arrange
        const string expected = "chat.message";

        // Act
        string actual = ChatEvent.GetSubscribeEventRoute(new SubscribeOptions());

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetSubscribeEventRouteShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => ChatEvent.GetSubscribeEventRoute(null));
    }
}
