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
/// </list>
/// </param>
/// <param name="Password">
/// The password used to authenticate the <see cref="UserAccount"/>.
/// <para>Validation rules:</para>
/// <list type="bullet">
///     <item>
///         <description>
///             Cannot be <c>null</c>, empty, or consist only of whitespace characters.
///         </description>
///     </item>
/// </list>
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserRequestDto(
    [Required(ErrorMessage = "Username is required.")]
    string Username,
    [Required(ErrorMessage = "Password is required.")]
    string Password);
