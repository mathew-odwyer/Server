// <copyright file="BadRequestException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when a request is invalid or cannot be processed due to client error.
/// </summary>
/// <remarks>
/// The <see cref="BadRequestException"/> is used to indicate a client-side error resulting in an HTTP 400 Bad Request response, typically caused by malformed input, missing data, or validation failures.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    public BadRequestException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public BadRequestException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
