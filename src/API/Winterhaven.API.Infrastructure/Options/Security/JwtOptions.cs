using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Infrastructure.Options.Security;

/// <summary>
///   Provides a set of options used for JSON Web Token (JWT) authentication.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class JwtOptions
{
    /// <summary>
    ///   Gets or sets a <see cref="double"/> that represents the life time of an access token in minutes.
    /// </summary>
    /// <value>
    ///   The <see cref="double"/> that represents the life time of an access token in minutes.
    /// </value>
    [Required]
    [Range(1, 15)]
    public required double AccessTokenExpiryMinutes { get; init; }

    /// <summary>
    ///   Gets or sets a <see cref="string"/> that represents the intended audience for the JWT.
    /// </summary>
    /// <value>
    ///   The <see cref="string"/> that represents the intended audience for the JWT.
    /// </value>
    [Required]
    public required string Audience { get; init; }

    /// <summary>
    ///   Gets or sets a <see cref="string"/> that represents the issuer of the JWT.
    /// </summary>
    /// <value>
    ///   The <see cref="string"/> that represents the issuer of the JWT.
    /// </value>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    ///   Gets or sets a <see cref="double"/> that represents the lifetime of the refresh token in days.
    /// </summary>
    /// <value>
    ///   The <see cref="double"/> that represents the lifetime of the refresh token in days.
    /// </value>
    [Required]
    [Range(1, 7)]
    public required double RefreshTokenExpiryDays { get; init; }

    /// <summary>
    ///   Gets or sets a <see cref="string"/> that represents the secret key used to sign and validate the JWT.
    /// </summary>
    /// <value>
    ///   The <see cref="string"/> that represents the secret key used to sign and validate the JWT.
    /// </value>
    [Required]
    public required string Secret { get; init; }

    /// <summary>
    ///   Gets or sets a value indicating whether the JWT audience should be validated during token validation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the JWT audience should be validated during token validation; otherwise, <c>false</c>.
    /// </value>
    [Required]
    public required bool ValidateAudience { get; init; }

    /// <summary>
    ///   Gets or sets a value indicating whether the JWT issuer should be validated during token validation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the JWT issuer should be validated during token validation; otherwise, <c>false</c>.
    /// </value>
    [Required]
    public required bool ValidateIssuer { get; init; }

    /// <summary>
    ///   Gets or sets a value indicating whether the JWT issuer signing key should be validated during token validation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the JWT issuer signing key should be validated during token validation; otherwise, <c>false</c>.
    /// </value>
    [Required]
    public required bool ValidateIssuerSigningKey { get; init; }

    /// <summary>
    ///   Gets a value indicating whether the signing key should be validated during JWT token validation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the signing key should be validated during JWT token validation; otherwise, <c>false</c>.
    /// </value>
    [Required]
    public required bool ValidateLifetime { get; init; }
}
