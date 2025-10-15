// <copyright file="ValidateUserRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public sealed record ValidateUserRequestDto(
    [property: JsonPropertyName("client_token")]
    [Required(ErrorMessage = "Client token is required.")]
    string ClientToken);
