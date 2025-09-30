// <copyright file="UpdatePlayerRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.DTOs.Players;

public sealed record UpdatePlayerRequestDto(
    string Name,
    int? X,
    int? Y);
