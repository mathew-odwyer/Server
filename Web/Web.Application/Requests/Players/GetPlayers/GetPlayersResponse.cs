// <copyright file="GetPlayersResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using System.Diagnostics.CodeAnalysis;
using Web.Application.DTOs.Players;

[ExcludeFromCodeCoverage]
public sealed record GetPlayersResponse(IEnumerable<PlayerDto> Players);
