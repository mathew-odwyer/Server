using System;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.Exceptions;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class MessageBusException : Exception
{
    /// <summary>
    /// </summary>
    public MessageBusException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public MessageBusException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MessageBusException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
