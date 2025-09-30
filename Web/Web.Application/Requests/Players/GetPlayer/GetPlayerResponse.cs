// <copyright file="GetPlayerResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using Web.Application.DTOs.Players;

public sealed record GetPlayerResponse(PlayerDto Player);
