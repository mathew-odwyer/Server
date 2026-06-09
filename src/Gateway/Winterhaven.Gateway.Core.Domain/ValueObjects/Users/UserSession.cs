using System;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

/// <summary>
///   Represents an authenticated user session.
/// </summary>
/// <param name="UserAccountId">
///   The unique identifier of the user account associated with the session.
/// </param>
/// <param name="AccessToken">
///   The access token used to authorize requests to protected resources.
/// </param>
/// <param name="ExpiresAt">
///   The date time offset that represents when the user session expires.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UserSession(
    Guid UserAccountId,
    string AccessToken,
    DateTimeOffset ExpiresAt);
