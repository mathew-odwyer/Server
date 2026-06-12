using System;

namespace Winterhaven.Gateway.Core.Domain.Exceptions;

/// <summary>
///   Represents an exception that is thrown when an issue occurrs when processing a chat message.
/// </summary>
public sealed class ChatMessageException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="ChatMessageException"/> class.
    /// </summary>
    public ChatMessageException()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ChatMessageException"/> class.
    /// </summary>
    /// <param name="message">
    ///   The message that describes the error.
    /// </param>
    public ChatMessageException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ChatMessageException"/> class.
    /// </summary>
    /// <param name="message">
    ///   The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a null reference ( <see langword="Nothing"/> in Visual Basic) if no inner exception is specified.
    /// </param>
    public ChatMessageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
