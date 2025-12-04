// <copyright file="GetMapRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Maps.GetMap;

using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Winterhaven.Core.Application.Exceptions;

public sealed class GetMapRequestHandler : IRequestHandler<GetMapRequest, GetMapResponse>
{
    private readonly IFileSystem fileSystem;

    private readonly ILogger<GetMapRequestHandler> logger;

    public GetMapRequestHandler(ILogger<GetMapRequestHandler> logger, IFileSystem fileSystem)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task<GetMapResponse> Handle(GetMapRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("Fetching map '{MapName}'", request.Name);

        string fullPath = Path.Combine("wwwroot", "Maps", $"{request.Name}.tmx");

        if (!this.fileSystem.File.Exists(fullPath))
        {
            throw new EntityNotFoundException("Map", fullPath);
        }

        byte[] bytes = await this.fileSystem.File.ReadAllBytesAsync(fullPath, cancellationToken).ConfigureAwait(false);

        if (bytes.Length == 0)
        {
            this.logger.LogWarning("Failed to read contents of map at path: '{FullPath}'", fullPath);
            throw new InvalidOperationException($"Failed to read contents of map at path: '{fullPath}'");
        }

        return new GetMapResponse(bytes);
    }
}
