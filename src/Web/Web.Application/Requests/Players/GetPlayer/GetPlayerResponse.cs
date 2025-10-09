// <copyright file="GetPlayerResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Diagnostics.CodeAnalysis;
using Web.Application.DTOs.Players;

/// <summary>
/// Represents a response that contains the details of an existing <see cref="Domain.Entities.Players.Player"/>.
/// </summary>
/// <param name="Player">
/// The <see cref="PlayerDto"/> that contains the details of the existing <see cref="Domain.Entities.Players.Player"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record GetPlayerResponse(PlayerDto Player);
