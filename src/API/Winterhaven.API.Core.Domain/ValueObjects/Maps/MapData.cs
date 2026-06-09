using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Core.Domain.ValueObjects.Maps;

/// <summary>
///   Represents a value object that provides data and information related to a map.
/// </summary>
/// <param name="Name">
///   The name of the map.
/// </param>
/// <param name="Data">
///   The raw data (as a string) of the map.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record MapData(
    string Name,
    string Data);
