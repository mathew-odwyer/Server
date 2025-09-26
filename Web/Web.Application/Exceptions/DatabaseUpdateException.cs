// <copyright file="DatabaseUpdateException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

public sealed class DatabaseUpdateException : Exception
{
    public DatabaseUpdateException()
    {
    }

    public DatabaseUpdateException(string message)
        : base(message)
    {
    }

    public DatabaseUpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
