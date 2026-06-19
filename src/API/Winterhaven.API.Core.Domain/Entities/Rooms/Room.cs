using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Core.Domain.Entities.Rooms;

/// <summary>
///   Represents a room/region within the world.
/// </summary>
[ExcludeFromCodeCoverage]
public class Room : EntityBase
{
    /// <summary>
    ///   Gets a <see cref="string"/> that represents the location of the map on disk.
    /// </summary>
    /// <value>
    ///   the location of the map on disk. This should be case-insensitive for compatibility with both Windows and Linux for development purposes.
    /// </value>
    public required string MapFilePath { get; init; }

    /// <summary>
    ///   Gets a <see cref="string"/> that represents the name of the map.
    /// </summary>
    /// <value>
    ///   The name of the map. This should typically be a normalized value to enforce it at the domain boundary.
    /// </value>
    public required string MapName { get; init; }
}
