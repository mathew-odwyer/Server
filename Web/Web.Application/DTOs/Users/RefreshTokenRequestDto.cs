// <copyright file="RefreshTokenRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record RefreshTokenRequestDto(
    [Required(ErrorMessage = "Refresh token is required.")]
    string RefreshToken);
