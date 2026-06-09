using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Players;

/// <summary>
///   Represents a player within the system.
/// </summary>
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
public sealed record GetPlayerRequestDto(
    string Name,
    double X,
    double Y);
