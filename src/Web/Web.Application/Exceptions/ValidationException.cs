// <copyright file="ValidationException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

/// <summary>
/// Represents an exception that is thrown when one or more validation failures occur.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">
    /// The errors.
    /// </param>
    public ValidationException(IDictionary<string, string[]>? errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified collection of validation failures.
    /// </summary>
    /// <param name="failures">
    /// Specifies an <see cref="IEnumerable{ValidationFailure}"/> representing the validation failures.
    /// </param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        this.Errors = failures
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage)
            .ToDictionary(
                x => x.Key,
                x => x.ToArray());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public ValidationException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.
    /// </param>
    public ValidationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets a dictionary of validation errors, where the key is the property name and the value is an array of error messages.
    /// </summary>
    /// <remarks>
    /// This property is populated when validation failures are provided to the constructor that takes an <see cref="IEnumerable{ValidationFailure}"/> as a parameter.
    /// </remarks>
    public IDictionary<string, string[]>? Errors { get; }
}
