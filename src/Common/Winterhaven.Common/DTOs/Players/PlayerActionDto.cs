using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Common.DTOs.Players;

/// <summary>
/// </summary>
/// <param name="Type"></param>
/// <param name="MoveX"></param>
/// <param name="MoveY"></param>
/// <param name="Identifier"></param>
[ExcludeFromCodeCoverage]
public sealed record PlayerActionDto(
    string Type,
    double MoveX,
    double MoveY,
    double Identifier);
