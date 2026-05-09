namespace Winterhaven.API.Infrastructure.Options.Maps;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed class MapStorageOptions
{
    [Required]
    public required string BasePath { get; init; }
}