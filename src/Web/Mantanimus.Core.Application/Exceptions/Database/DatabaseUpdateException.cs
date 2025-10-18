// <copyright file="DatabaseUpdateException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Exceptions.Database;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when a database update operation fails.
/// </summary>
/// <seealso cref="Exception" />
[ExcludeFromCodeCoverage]
public sealed class DatabaseUpdateException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateException"/> class.
    /// </summary>
    public DatabaseUpdateException()
        : base("A database update exception occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public DatabaseUpdateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdateException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public DatabaseUpdateException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
