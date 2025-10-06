// <copyright file="DatabaseUpdateConcurrencyException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Database;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when a database update operation fails due to a concurrency conflict.
/// </summary>
/// <remarks>
/// This exception typically occurs when multiple operations attempt to modify the
/// same database record simultaneously, resulting in a conflict between the expected
/// and actual data states.
/// <para>
/// It is commonly used to indicate that an update or delete operation could not be
/// completed because the underlying data has changed since it was last read.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class DatabaseUpdateConcurrencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateConcurrencyException"/> class.
    /// </summary>
    public DatabaseUpdateConcurrencyException()
        : base("A database update concurrency conflict occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateConcurrencyException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public DatabaseUpdateConcurrencyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateConcurrencyException"/> class.
    /// </summary>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public DatabaseUpdateConcurrencyException(Exception innerException)
        : base("A database update concurrency conflict occurred.", innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateConcurrencyException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public DatabaseUpdateConcurrencyException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
