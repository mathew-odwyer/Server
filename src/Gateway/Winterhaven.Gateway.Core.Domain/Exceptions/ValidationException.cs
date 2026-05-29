using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Gateway.Core.Domain.Exceptions;

/// <summary>
///   Represents an exception that is thrown when one or more validation failures occur.
/// </summary>
/// <seealso cref="Exception"/>
[ExcludeFromCodeCoverage]
public sealed class ValidationException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">
    ///   The message that describes the error.
    /// </param>
    public ValidationException(string? message)
        : base(message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">
    ///   The errors that describe the reason for the <see cref="ValidationException"/>.
    /// </param>
    public ValidationException(IReadOnlyDictionary<string, string[]>? errors)
        : base("One or more validation failures have occurred.") => Errors = (IDictionary<string, string[]>?)errors;

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    ///   The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public ValidationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    ///   Gets the errors that describe the reason for the <see cref="ValidationException"/>.
    /// </summary>
    /// <value>
    ///   The errors that describe the reason for the <see cref="ValidationException"/>.
    /// </value>
    public IDictionary<string, string[]>? Errors { get; }
}
