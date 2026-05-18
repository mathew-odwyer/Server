namespace Winterhaven.Gateway.Core.Domain.Events.Sessions;

using System;

public sealed class SessionAuthenticatedEventArgs : EventArgs
{
    public SessionAuthenticatedEventArgs(string username, TimeSpan accessTokenExpiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        this.Username = username;
        this.AccessTokenExpiry = accessTokenExpiry;
    }

    public TimeSpan AccessTokenExpiry { get; init; }

    public string Username { get; init; }
}