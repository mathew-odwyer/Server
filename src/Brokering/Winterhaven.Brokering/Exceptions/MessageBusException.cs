using System;

namespace Winterhaven.Brokering.Exceptions;

/// <summary>
/// </summary>
public sealed class MessageBusException : Exception
{
    /// <summary>
    /// </summary>
    public MessageBusException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message">
    /// </param>
    public MessageBusException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message">
    /// </param>
    /// <param name="innerException">
    /// </param>
    public MessageBusException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
