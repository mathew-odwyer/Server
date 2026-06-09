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
    public MessageBusException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// </summary>
    public MessageBusException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
