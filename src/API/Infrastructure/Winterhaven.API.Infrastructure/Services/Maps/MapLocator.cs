namespace Winterhaven.API.Infrastructure.Services.Maps;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Services.Maps;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Core.Domain.ValueObjects.Maps;
using Winterhaven.API.Infrastructure.Options.Maps;

internal sealed class MapLocator : IMapLocator
{
    private readonly IFileSystem fileSystem;

    private readonly ILogger<MapLocator> logger;

    private readonly IOptions<MapStorageOptions> options;

    public MapLocator(ILogger<MapLocator> logger, IFileSystem fileSystem, IOptions<MapStorageOptions> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<MapData> LocateMapDataAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        this.logger.LogDebug("Locating map '{MapName}'", name);

#pragma warning disable CA1308 // Normalize strings to uppercase
        // This is required to ensure that on both windows and Linux operating systems the file path can be located regardless of case-sensitivity.
        string fullPath = Path.Combine(this.options.Value.BasePath, $"{name.ToLowerInvariant()}.tmx");
#pragma warning restore CA1308 // Normalize strings to uppercase

        if (!this.fileSystem.File.Exists(fullPath))
        {
            // Do not leak full file-system path to the map file, instead log an error.
            this.logger.LogError("Failed to locate map at path: '{Path}'", fullPath);
            throw new ResourceNotFoundException("Map", name);
        }

        string data = await this.fileSystem.File.ReadAllTextAsync(fullPath, cancellationToken).ConfigureAwait(false);

        // If there is no content in the TMX file, that is not a valid TMX file - so throw an error (500).
        if (data.Length == 0)
        {
            this.logger.LogWarning("Failed to read contents of map at path: '{FullPath}'", fullPath);
            throw new InvalidOperationException($"Failed to read contents of map at path: '{fullPath}'");
        }

        return new MapData(
            Name: name.ToUpperInvariant().Replace("_", " ", StringComparison.OrdinalIgnoreCase),
            Data: data);
    }
}