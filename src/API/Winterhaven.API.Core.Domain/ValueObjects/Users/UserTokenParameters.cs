using System;

namespace Winterhaven.API.Core.Domain.ValueObjects.Users;

/// <summary>
///   Represents the parameters used to generate a user token for a user account.
/// </summary>
/// <param name="UserAccountId">
///   Specifies a <see cref="string"/> representing the unique identifier of the user account for which the JWT is being generated.
/// </param>
/// <param name="Username">
///   Specifies a <see cref="string"/> representing the username associated with the user account.
/// </param>
public sealed record UserTokenParameters(
    Guid UserAccountId,
    string Username);