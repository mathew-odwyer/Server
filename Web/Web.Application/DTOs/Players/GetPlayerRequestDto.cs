// <copyright file="GetPlayerRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Players;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record GetPlayerRequestDto(
    [Required(ErrorMessage = "Name is required.")]
    string Name);
