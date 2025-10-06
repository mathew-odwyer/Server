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

public sealed class PlayerController : ApiControllerBase
{
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var request = new GetPlayersRequest(
            UserAccountId: this.User.FindFirstValue("identifier")!);

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.Ok(response);
    }

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
