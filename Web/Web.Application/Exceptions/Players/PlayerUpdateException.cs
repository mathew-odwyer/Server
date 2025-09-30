// <copyright file="PlayerUpdateException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Players;

public sealed class PlayerUpdateException : Exception
{
    public PlayerUpdateException()
        : base("Failed to update player due to a database error.")
    {
    }

    public PlayerUpdateException(string message)
        : base(message)
    {
    }

    public PlayerUpdateException(string userAccountId, string name, Exception? innerException)
    : base($"Failed to update player with name: '{name}' for user with ID: '{userAccountId}'", innerException)
    {
    }

    public PlayerUpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
