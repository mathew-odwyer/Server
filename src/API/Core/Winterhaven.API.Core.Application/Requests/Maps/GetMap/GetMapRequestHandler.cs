namespace Winterhaven.API.Core.Application.Requests.Maps.GetMap;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Services.Maps;

/// <summary>
///   Represents a request handler that is used to fetch an existing map.
/// </summary>
public sealed class GetMapRequestHandler : IRequestHandler<GetMapRequest, GetMapResponse>
{
    /// <summary>
    ///   The logger.
    /// </summary>
    private readonly ILogger<GetMapRequestHandler> logger;

    /// <summary>
    ///   The map locator.
    /// </summary>
    private readonly IMapLocator mapLocator;

    /// <summary>
    ///   Initializes a new instance of the <see cref="GetMapRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    ///   The logger.
    /// </param>
    /// <param name="mapLocator">
    ///   The map locator used to fetch the map.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when one of the following parameters is <c>null</c>:
    ///   <list type="bullet">
    ///     <item>
    ///       <description><paramref name="logger"/></description>
    ///     </item>
    ///   </list>
    /// </exception>
    public GetMapRequestHandler(ILogger<GetMapRequestHandler> logger, IMapLocator mapLocator)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.mapLocator = mapLocator ?? throw new ArgumentNullException(nameof(mapLocator));
    }

    /// <inheritdoc/>
    public async Task<GetMapResponse> Handle(GetMapRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogDebug("Fetching map '{MapName}'", request.Name);
        var mapData = await this.mapLocator.LocateMapDataAsync(request.Name, cancellationToken).ConfigureAwait(false);

        return new GetMapResponse(
            Name: mapData.Name,
            Data: mapData.Data);
    }
}