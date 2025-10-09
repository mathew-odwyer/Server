// <copyright file="PlayerCreateException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Players;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an exception that is thrown when an error occurs while creating a new player.
/// </summary>
/// <remarks>
/// The <see cref="PlayerCreateException"/> is used to indicate that a player creation operation has failed, typically due to a database error or data consistency issue encountered during the persistence process.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class PlayerCreateException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerCreateException"/> class.
    /// </summary>
    public PlayerCreateException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerCreateException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public PlayerCreateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerCreateException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public PlayerCreateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerCreateException"/> class.
    /// </summary>
    /// <param name="userAccountId">
    /// The user account identifier associated with the player.
    /// </param>
    /// <param name="name">The name of the player.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public PlayerCreateException(string userAccountId, string name, Exception? innerException)
        : base($"Failed to create player with name: '{name}' for user with ID: '{userAccountId}'", innerException)
    {
    }
}
