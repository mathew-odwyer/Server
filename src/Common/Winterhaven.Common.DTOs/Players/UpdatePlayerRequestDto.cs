namespace Winterhaven.Common.DTOs.Players;

using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents the data transfer object used to update an existing player.
/// </summary>
/// <param name="X">
///   The current X-coordinate of the player.
/// </param>
/// <param name="Y">
///   The current Y-coordinate of the player.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UpdatePlayerRequestDto(
    double? X,
    double? Y);