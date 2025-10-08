// <copyright file="PlayerController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Controllers.Players;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTOs.Players;
using Web.Application.Requests.Players.CreatePlayer;
using Web.Application.Requests.Players.DeletePlayer;
using Web.Application.Requests.Players.GetPlayer;
using Web.Application.Requests.Players.GetPlayers;
using Web.Application.Requests.Players.UpdatePlayer;
using Web.Domain.Entities.Players;

/// <summary>
/// Provides API endpoints for managing <see cref="Player"/> entities.
/// </summary>
public sealed class PlayerController : ApiControllerBase
{
    /// <summary>
    /// Creates a new player associated with the authenticated user.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="CreatePlayerRequestDto"/> containing the data required to create a player.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a <see cref="NoContentResult"/> when the player is successfully created.
    /// </returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePlayerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new CreatePlayerRequest(
            Name: requestDto.Name,
            UserAccountId: this.User.FindFirstValue("identifier")!);

        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.NoContent();
    }

    /// <summary>
    /// Deletes an existing player associated with the authenticated user.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="DeletePlayerRequestDto"/> containing the data required to delete a player.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a <see cref="NoContentResult"/> when the player is successfully deleted.
    /// </returns>
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete([FromBody] DeletePlayerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new DeletePlayerRequest(
            Name: requestDto.Name,
            UserAccountId: this.User.FindFirstValue("identifier")!);

        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.NoContent();
    }

    /// <summary>
    /// Retrieves a specific player associated with the authenticated user.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="GetPlayerRequestDto"/> containing the criteria used to retrieve a player.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns an <see cref="OkObjectResult"/> containing the retrieved player.
    /// </returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] GetPlayerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new GetPlayerRequest(
            Name: requestDto.Name,
            UserAccountId: this.User.FindFirstValue("identifier")!);

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.Ok(response);
    }

    /// <summary>
    /// Retrieves all players associated with the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns an <see cref="OkObjectResult"/> containing the list of players.
    /// </returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var request = new GetPlayersRequest(
            UserAccountId: this.User.FindFirstValue("identifier")!);

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.Ok(response);
    }

    /// <summary>
    /// Updates an existing player associated with the authenticated user.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="UpdatePlayerRequestDto"/> containing the updated player data.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// Returns a <see cref="NoContentResult"/> when the player is successfully updated.
    /// </returns>
    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdatePlayerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new UpdatePlayerRequest(
            UserAccountId: this.User.FindFirstValue("identifier")!,
            Name: requestDto.Name,
            X: requestDto.X,
            Y: requestDto.Y);

        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.NoContent();
    }
}
