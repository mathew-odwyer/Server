namespace Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

using System;

public sealed record UserSession(
    string Username,
    string AccessToken,
    string RefreshToken,
    TimeSpan AccessTokenExpiry);