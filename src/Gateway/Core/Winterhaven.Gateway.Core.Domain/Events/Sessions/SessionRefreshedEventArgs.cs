namespace Winterhaven.Gateway.Core.Domain.Events.Sessions;

using System;

public sealed class SessionRefreshedEventArgs : EventArgs
{
    public SessionRefreshedEventArgs(string username, TimeSpan accessTokenExpiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        this.Username = username;
        this.AccessTokenExpiry = accessTokenExpiry;
    }

    public TimeSpan AccessTokenExpiry { get; init; }

    public string Username { get; init; }
}