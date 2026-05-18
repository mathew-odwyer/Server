namespace Winterhaven.API.Core.Domain.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents an exception that is thrown when access to a resource is forbidden.
/// </summary>
/// <seealso cref="Exception"/>
[ExcludeFromCodeCoverage]
public sealed class ForbiddenAccessException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="ForbiddenAccessException"/> class.
    /// </summary>
    public ForbiddenAccessException()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ForbiddenAccessException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">
    ///   The message that describes the error.
    /// </param>
    public ForbiddenAccessException(string? message)
        : base(message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ForbiddenAccessException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    ///   The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public ForbiddenAccessException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}