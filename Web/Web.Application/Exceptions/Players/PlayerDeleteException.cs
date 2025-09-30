// <copyright file="PlayerDeleteException.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Exceptions.Players;

public sealed class PlayerDeleteException : Exception
{
    public PlayerDeleteException()
        : base("Failed to delete player due to a database error.")
    {
    }

    public PlayerDeleteException(string message)
        : base(message)
    {
    }

    public PlayerDeleteException(string userAccountId, string name, Exception? innerException)
        : base($"Failed to delete player with name: '{name}' for user with ID: '{userAccountId}'", innerException)
    {
    }

    public PlayerDeleteException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
