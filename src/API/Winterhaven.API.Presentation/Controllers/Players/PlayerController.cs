using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Winterhaven.API.Core.Application.Requests.Players.GetPlayer;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.Common.DTOs.Players;

namespace Winterhaven.API.Presentation.Controllers.Players;

/// <summary>
///   Provides API endpoints for managing player entities.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PlayerController : ApiControllerBase
{
    /// <summary>
    ///   Retrieves a specific player associated with the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">
    ///   Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///   Returns an <see cref="OkObjectResult"/> containing the retrieved player.
    /// </returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var request = new GetPlayerRequest();
        var response = await Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return Ok(Mapper.Map<GetPlayerResponseDto>(response));
    }

    /// <summary>
    ///   Updates an existing player associated with the authenticated user.
    /// </summary>
    /// <param name="requestDto">
    ///   Specifies a <see cref="UpdatePlayerRequestDto"/> containing the updated player data.
    /// </param>
    /// <param name="cancellationToken">
    ///   Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///   Returns a <see cref="NoContentResult"/> when the player is successfully updated.
    /// </returns>
    [HttpPatch]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update([FromBody] UpdatePlayerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = Mapper.Map<UpdatePlayerRequest>(requestDto);
        await Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}