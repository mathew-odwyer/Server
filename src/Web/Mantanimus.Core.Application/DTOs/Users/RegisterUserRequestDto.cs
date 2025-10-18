// <copyright file="RegisterUserRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Represents the data transfer object used to register a new user account.
/// </summary>
/// <param name="EmailAddress">
/// The email address associated with the new <see cref="UserAccount"/>.
/// <list type="bullet">
///     <item>
///         <description>
///             Cannot be <c>null</c> or empty.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must be a valid email address format (e.g., <c>name@example.com</c>).
///         </description>
///     </item>
/// </list>
/// </param>
/// <param name="Username">
/// The desired username for the new <see cref="UserAccount"/>.
/// <para>Validation rules:</para>
/// <list type="bullet">
///     <item>
///         <description>
///             Cannot be <c>null</c>, empty, or consist only of whitespace characters.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must be at least 3 characters long.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must be no more than 12 characters long.
///         </description>
///     </item>
///     <item>
///         <description>
///             Can only contain alphanumeric characters, hyphens (<c>-</c>), or underscores (<c>_</c>).
///         </description>
///     </item>
/// </list>
/// </param>
/// <param name="Password">
/// The password used to secure the new <see cref="UserAccount"/>.
/// <para>Validation rules:</para>
/// <list type="bullet">
///     <item>
///         <description>
///             Cannot be <c>null</c> or empty.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must be at least 12 characters long.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must contain at least one uppercase letter.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must contain at least one lowercase letter.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must contain at least one numeric character.
///         </description>
///     </item>
///     <item>
///         <description>
///             Must contain at least one special (non-alphanumeric) character.
///         </description>
///     </item>
/// </list>
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RegisterUserRequestDto(
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Email address is invalid.")]
    string EmailAddress,

    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(12, ErrorMessage = "Username must be at least 12 characters.")]
    string Username,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
    string Password);
