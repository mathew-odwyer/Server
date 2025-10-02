// <copyright file="GetPlayerResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Diagnostics.CodeAnalysis;
using Web.Application.DTOs.Players;

[ExcludeFromCodeCoverage]
public sealed record GetPlayerResponse(PlayerDto Player);
