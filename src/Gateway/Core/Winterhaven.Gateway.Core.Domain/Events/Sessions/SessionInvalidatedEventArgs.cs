namespace Winterhaven.Gateway.Core.Domain.Events.Sessions;

using System;

public sealed class SessionInvalidatedEventArgs : EventArgs
{
    public SessionInvalidatedEventArgs(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        this.Username = username;
    }

    public string Username { get; init; }
}