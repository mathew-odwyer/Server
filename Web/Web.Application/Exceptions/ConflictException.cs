// <copyright file="ConflictException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

public sealed class ConflictException : Exception
{
    public ConflictException()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(IDictionary<string, string> errors)
       : base("One or more conflicts occurred.")
    {
        this.Errors = errors;
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public IDictionary<string, string>? Errors { get; }
}
