// <copyright file="DatabaseUpdateConcurrencyException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Database;

public sealed class DatabaseUpdateConcurrencyException : Exception
{
    public DatabaseUpdateConcurrencyException()
        : base("A database update concurrency conflict occurred.")
    {
    }

    public DatabaseUpdateConcurrencyException(string message)
        : base(message)
    {
    }

    public DatabaseUpdateConcurrencyException(Exception innerException)
        : base("A database update concurrency conflict occurred.", innerException)
    {
    }

    public DatabaseUpdateConcurrencyException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
