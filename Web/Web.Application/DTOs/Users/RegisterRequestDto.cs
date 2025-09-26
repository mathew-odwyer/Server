// <copyright file="RegisterRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Data transfer object for user registration requests.
/// </summary>
/// <param name="EmailAddress">
/// The user's email address. Must be provided and conform to a valid email format.
/// </param>
/// <param name="Username">
/// The desired username for the new account. Must be provided.
/// </param>
/// <param name="Password">
/// The password for the new account. Must be provided and at least 12 characters long.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RegisterUserRequestDto(
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Email address is invalid.")]
    string EmailAddress,

    [Required(ErrorMessage = "Username is required.")]
    string Username,

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
    string Password);
