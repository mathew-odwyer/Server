using System;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Players;

/// <summary>
///   Represents the data transfer object used to update an existing player.
/// </summary>
/// <param name="PlayerId">
///   The identifier of the player to update.
/// </param>
/// <param name="X">
///   The current X-coordinate of the player.
/// </param>
/// <param name="Y">
///   The current Y-coordinate of the player.
/// </param>
/// <param name="RoomId">
///   The identifier of the room the player is in.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UpdatePlayerRequestDto(
    Guid PlayerId,
    double? X,
    double? Y,
    Guid RoomId);
