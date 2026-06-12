using System;

namespace Winterhaven.Common.Events.Chat;

/// <summary>
///   Enumerates the available chat emote types.
/// </summary>
public enum ChatEmoteType
{
    /// <summary>
    ///   No emote.
    /// </summary>
    None = 0,

    /// <summary>
    ///   The ellipsis emote (...).
    /// </summary>
    Ellipsis = 1,

    /// <summary>
    ///   The exclaim emote (!).
    /// </summary>
    Exclaim = 2,

    /// <summary>
    ///   The heart emote
    /// </summary>
    Heart = 3,

    /// <summary>
    ///   The question emote (?).
    /// </summary>
    Question = 4,
}

/// <summary>
///   Represents an event that is published when a player sends a chat message.
/// </summary>
public sealed record ChatEvent : IEvent
{
    /// <summary>
    ///   Gets the identifier of the player that sent the message (or <c>null</c> if the server sent a message).
    /// </summary>
    /// <value>
    ///   The identifier of the player that sent the message (or <c>null</c> if the server sent a message).
    /// </value>
    public Guid? SenderId { get; init; }

    /// <summary>
    ///   Gets the message to be sent.
    /// </summary>
    /// <value>
    ///   The message to be sent.
    /// </value>
    public required string Message { get; init; }

    /// <summary>
    ///   Gets the emote type.
    /// </summary>
    /// <value>
    ///   The emote type.
    /// </value>
    public ChatEmoteType EmoteType { get; init; }

    /// <inheritdoc/>
    public static string GetPublishEventRoute(PublishOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return "chat.message";
    }

    /// <inheritdoc/>
    public static string GetSubscribeEventRoute(SubscribeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return "chat.message";
    }
}
