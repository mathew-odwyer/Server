// <copyright file="LoginUserRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents the data transfer object used to authenticate an existing <see cref="UserAccount"/>.
/// </summary>
/// <param name="Username">
/// The username associated with the <see cref="UserAccount"/>.
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
/// The password used to authenticate the <see cref="UserAccount"/>.
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
public sealed record LoginUserRequestDto(
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3)]
    [MaxLength(12)]
    string Username,
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(12)]
    string Password);
