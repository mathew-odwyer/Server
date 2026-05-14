namespace Winterhaven.Common.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when a request is made without proper authentication or
/// when authentication fails.
/// </summary>
/// <seealso cref="Exception"/>
[ExcludeFromCodeCoverage]
public sealed class AuthorizationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
    /// </summary>
    public AuthorizationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AuthorizationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no
    /// inner exception is specified.
    /// </param>
    public AuthorizationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}