// <copyright file="DatabaseUpdateException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Database;

public sealed class DatabaseUpdateException : Exception
{
    public DatabaseUpdateException()
        : base("A database update exception occurred.")
    {
    }

    public DatabaseUpdateException(string message)
        : base(message)
    {
    }

    public DatabaseUpdateException(Exception innerException)
        : base("A database update exception occurred.", innerException)
    {
    }

    public DatabaseUpdateException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
