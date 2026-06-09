using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Players;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record PlayerActionDto(
    string Type,
    double MoveX,
    double MoveY,
    double Identifier);
