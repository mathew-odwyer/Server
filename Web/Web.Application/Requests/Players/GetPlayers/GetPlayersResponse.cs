// <copyright file="GetPlayersResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using System.Diagnostics.CodeAnalysis;
using Web.Application.DTOs.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a response that contains the details of all existing players for the current <see cref="UserAccount"/>.
/// </summary>
/// <param name="Players">
/// The players associated with the current <see cref="UserAccount"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record GetPlayersResponse(IEnumerable<PlayerDto> Players);
