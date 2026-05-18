namespace Winterhaven.Common.DTOs.Maps;

/// <summary>
///   Represents the data transfer object used to fetch a map.
/// </summary>
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