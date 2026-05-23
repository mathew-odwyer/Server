namespace Winterhaven.Gateway.Core.Domain.Events.Sessions;

using System;

public sealed class SessionInvalidatedEventArgs : EventArgs
{
    public SessionInvalidatedEventArgs(Guid userAccountId, string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        this.UserAccountId = userAccountId;
        this.Username = username;
    }

    public Guid UserAccountId { get; init; }

    public string Username { get; init; }
}