// <copyright file="EntityNotFoundException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when an entity is not found.
/// </summary>
/// <seealso cref="Exception" />
[ExcludeFromCodeCoverage]
public sealed class EntityNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    public EntityNotFoundException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public EntityNotFoundException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified entity name and key.
    /// </summary>
    /// <param name="name">
    /// Specifies a <see cref="string"/> representing the name of the entity that was not found.
    /// </param>
    /// <param name="key">
    /// Specifies an <see cref="object"/> representing the key of the entity that was not found.
    /// </param>
    public EntityNotFoundException(string name, object key)
        : base($"Entity '{name}' ({key}) was not found.")
    {
    }
}
