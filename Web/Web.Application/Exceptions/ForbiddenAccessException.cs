// <copyright file="ForbiddenAccessException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when access to a resource is forbidden.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ForbiddenAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class.
    /// </summary>
    public ForbiddenAccessException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">
    /// Specifies a <see cref="string"/> representing the error message.
    /// </param>
    public ForbiddenAccessException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// Specifies a <see cref="string"/> representing the error message.
    /// </param>
    /// <param name="innerException">
    /// Specifies an <see cref="Exception"/> that is the cause of the current exception.
    /// </param>
    public ForbiddenAccessException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
