using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Infrastructure.Options.Maps;

/// <summary>
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class MapStorageOptions
{
    /// <summary>
    /// </summary>
    [Required]
    public required string BasePath { get; init; }
}
