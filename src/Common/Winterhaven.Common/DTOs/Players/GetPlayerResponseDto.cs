using System;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Players;

/// <summary>
///   Represents a DTO that contains the details of a player.
/// </summary>
/// <param name="Id">
///   The unique identifier of the player.
/// </param>
/// <param name="Name">
///   The unique name of the player.
/// </param>
/// <param name="X">
///   The current X-coordinate of the player.
/// </param>
/// <param name="Y">
///   The current Y-coordinate of the player.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record GetPlayerResponseDto(
    Guid Id,
    string Name,
    double X,
    double Y);
