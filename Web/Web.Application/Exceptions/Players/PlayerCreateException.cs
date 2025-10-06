// <copyright file="PlayerCreateException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Players;

public sealed class PlayerCreateException : Exception
{
    public PlayerCreateException()
    {
    }

    public PlayerCreateException(string message)
        : base(message)
    {
    }

    public PlayerCreateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public PlayerCreateException(string userAccountId, string name, Exception? innerException)
        : base($"Failed to create player with name: '{name}' for user with ID: '{userAccountId}'", innerException)
    {
    }
}
