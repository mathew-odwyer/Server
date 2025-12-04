// <copyright file="MapController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Presentation.Controllers.Maps;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Winterhaven.Core.Application.DTOs.Maps;
using Winterhaven.Core.Application.Requests.Maps.GetMap;

public sealed class MapController : ApiControllerBase
{
    private const string ContentType = "text/xml";

    [HttpGet]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public async Task<IActionResult> Get([FromQuery] GetMapRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<GetMapRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.File(response.Data, ContentType);
    }
}
