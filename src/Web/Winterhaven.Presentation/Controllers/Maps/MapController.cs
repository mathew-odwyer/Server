// <copyright file="MapController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Presentation.Controllers.Maps;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Winterhaven.Core.Application.DTOs.Maps;
using Winterhaven.Core.Application.Requests.Maps.GetMap;
using Winterhaven.Presentation.Authentication;

/// <summary>
/// Provides API endpoints for handling maps.
/// </summary>
/// <seealso cref="ApiControllerBase" />
public sealed class MapController : ApiControllerBase
{
    private const string ContentType = "text/xml";

    /// <summary>
    /// Retrieves a specific map associated with the specified <see cref="GetMapRequestDto.Name"/>.
    /// </summary>
    /// <param name="requestDto">
    /// The request data transfer object used to fetch the map.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a file containing the map data.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The specified <paramref name="requestDto"/> parameter cannot be null.
    /// </exception>
    [HttpGet]
    [Authorize(AuthenticationSchemes = WinterhavenBearerDefaults.ServerAuthenticationScheme)]
    public async Task<IActionResult> Get([FromQuery] GetMapRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<GetMapRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.File([.. response.Data], ContentType);
    }
}
