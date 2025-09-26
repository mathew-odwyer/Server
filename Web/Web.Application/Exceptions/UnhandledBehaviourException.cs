// <copyright file="UnhandledBehaviourException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions;

public sealed class UnhandledBehaviourException : Exception
{
    public UnhandledBehaviourException()
    {
    }

    public UnhandledBehaviourException(string message)
        : base(message)
    {
    }

    public UnhandledBehaviourException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
