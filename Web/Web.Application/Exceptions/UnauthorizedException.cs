// <copyright file="UnauthorizedException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a request is made without proper authentication or when authentication fails.
/// </summary>
/// <remarks>
/// The <see cref="UnauthorizedException"/> is used to indicate an HTTP 401 Unauthorized response, typically occurring when a user is not authenticated or the provided credentials are invalid.
/// </remarks>
public sealed class UnauthorizedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    public UnauthorizedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public UnauthorizedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public UnauthorizedException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
