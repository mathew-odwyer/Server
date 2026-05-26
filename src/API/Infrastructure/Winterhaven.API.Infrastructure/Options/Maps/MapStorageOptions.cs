using System.ComponentModel.DataAnnotations;

namespace Winterhaven.API.Infrastructure.Options.Maps;

/// <summary>
/// </summary>
public sealed class MapStorageOptions
{
    /// <summary>
    /// </summary>
    [Required]
    public required string BasePath { get; init; }
}