namespace Winterhaven.Common.DTOs.Maps;

using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents the data transfer object used to fetch a map.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record GetMapRequestDto
{
    /// <summary>
    ///   Gets the name of the map to fetch.
    /// </summary>
    /// <value>
    ///   The name of the map to fetch.
    /// </value>
    public required string Name { get; init; }
}