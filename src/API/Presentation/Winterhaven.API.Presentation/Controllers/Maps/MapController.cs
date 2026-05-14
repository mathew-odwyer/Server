namespace Winterhaven.API.Presentation.Controllers.Maps;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Requests.Maps.GetMap;
using Winterhaven.API.Presentation.Authentication;
using Winterhaven.API.Common.DTOs.Maps;

/// <summary>
/// Provides API endpoints for handling maps.
/// </summary>
/// <seealso cref="ApiControllerBase"/>
[ExcludeFromCodeCoverage]
public sealed class MapController : ApiControllerBase
{
    /// <summary>
    /// Retrieves a specific map associated with the specified <see cref="GetMapRequestDto.Name"/>.
    /// </summary>
    /// <param name="requestDto">The request data transfer object used to fetch the map.</param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>Returns a file containing the map data.</returns>
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

        return this.Ok(response);
    }
}