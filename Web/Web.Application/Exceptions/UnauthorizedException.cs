// <copyright file="UnauthorizedException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
