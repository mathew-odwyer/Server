// <copyright file="CreatePlayerRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Players;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the data transfer object used to create a new <see cref="Player"/>.
/// </summary>
/// <param name="Name">
/// The name of the player.
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
[ExcludeFromCodeCoverage]
public sealed record CreatePlayerRequestDto(
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(3)]
    [MaxLength(12)]
    string Name);
