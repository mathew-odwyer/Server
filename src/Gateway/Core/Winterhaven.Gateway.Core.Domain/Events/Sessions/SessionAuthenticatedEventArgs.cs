namespace Winterhaven.Gateway.Core.Domain.Events.Sessions;

using System;

public sealed class SessionAuthenticatedEventArgs : EventArgs
{
    public string Username { get; init; } = string.Empty;

    public TimeSpan AccessTokenExpiry { get; init; }
}