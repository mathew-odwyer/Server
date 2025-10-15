// <copyright file="ValidateUserRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;

public sealed record ValidateUserRequestDto(
    [Required(ErrorMessage = "Client token is required.")]
    string ClientToken);
