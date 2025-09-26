// <copyright file="JwtOptions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Options.Security;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines a set of options used for JSON Web Token (JWT) authentication.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class JwtOptions
{
    public required double AccessTokenExpiryMinutes { get; init; }

    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the intended audience for the JWT.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> that represents the intended audience for the JWT.
    /// </value>
    public required string Audience { get; init; }

    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the issuer of the JWT.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> that represents the issuer of the JWT.
    /// </value>
    public required string Issuer { get; init; }

    public required double RefreshTokenExpiryDays { get; init; }

    /// <summary>
    /// Gets or sets a <see cref="string"/> that represents the secret key used to sign and validate the JWT.
    /// </summary>
    /// <value>
    /// The <see cref="string"/> that represents the secret key used to sign and validate the JWT.
    /// </value>
    public required string Secret { get; init; }

    public required bool ValidateAudience { get; init; }

    public required bool ValidateIssuer { get; init; }

    public required bool ValidateIssuerSigningKey { get; init; }

    public required bool ValidateLifetime { get; init; }
}
