// <copyright file="GetPlayersResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using Web.Application.DTOs.Players;

public sealed record GetPlayersResponse(IEnumerable<PlayerDto> Players);
