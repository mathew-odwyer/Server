using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Core.Domain.Entities.Rooms;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public class Room : EntityBase
{
    /// <summary>
    /// </summary>
    public required string MapFilePath { get; init; }

    /// <summary>
    /// </summary>
    public required string MapName { get; init; }
}
