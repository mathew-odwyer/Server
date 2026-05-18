namespace Winterhaven.API.Core.Domain.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents an exception that is thrown when a conflict occurs during a request, typically due to a resource already existing or a state mismatch.
/// </summary>
/// <seealso cref="Exception"/>
[ExcludeFromCodeCoverage]
public sealed class ConflictException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    public ConflictException()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    /// <param name="message">
    ///   The message that describes the error.
    /// </param>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    /// <param name="message">
    ///   The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}