namespace Winterhaven.Common.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when a resource is not found.
/// </summary>
/// <seealso cref="Exception"/>
[ExcludeFromCodeCoverage]
public sealed class ResourceNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
    /// </summary>
    public ResourceNotFoundException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class with a
    /// specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ResourceNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class with a
    /// specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no
    /// inner exception is specified.
    /// </param>
    public ResourceNotFoundException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class with a
    /// specified entity name and key.
    /// </summary>
    /// <param name="name">
    /// Specifies a <see cref="string"/> representing the name of the resource that was not found.
    /// </param>
    /// <param name="key">
    /// Specifies an <see cref="object"/> representing the key of the resource that was not found.
    /// </param>
    public ResourceNotFoundException(string name, object key)
        : base($"Entity '{name}' ({key}) was not found.")
    {
        this.Name = name;
        this.Key = key;
    }

    /// <summary>
    /// Gets the name of the resource that was not found
    /// </summary>
    /// <value>The name of the resource that was not found</value>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the key (usually an identifier) of the resource that was not found
    /// </summary>
    /// <value>The key (usually an identifier) of the resource that was not found</value>
    public object? Key { get; init; }
}