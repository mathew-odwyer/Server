// <copyright file="DatabaseUpdateConcurrencyException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

public sealed class DatabaseUpdateConcurrencyException : Exception
{
    public DatabaseUpdateConcurrencyException()
    {
    }

    public DatabaseUpdateConcurrencyException(string message)
        : base(message)
    {
    }

    public DatabaseUpdateConcurrencyException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
