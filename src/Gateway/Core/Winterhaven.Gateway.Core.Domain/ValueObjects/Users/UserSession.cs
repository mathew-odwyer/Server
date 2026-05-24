namespace Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

using System;

public sealed record UserSession(
    Guid UserAccountId,
    string Username,
    string AccessToken,
    string RefreshToken,
    TimeSpan AccessTokenExpiry);