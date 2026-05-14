namespace Winterhaven.API.Common.DTOs.Players;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a player within the system.
/// </summary>
/// <param name="Name">The unique name of the <see cref="Player"/>.</param>
/// <param name="X">The current X-coordinate of the <see cref="Player"/>.</param>
/// <param name="Y">The current Y-coordinate of the <see cref="Player"/>.</param>
[ExcludeFromCodeCoverage]

public sealed record GetPlayerRequestDto(
    string Name,
    double X,
    double Y);