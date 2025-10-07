// <copyright file="UserAccountController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Controllers.Users;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTOs.Users;
using Web.Application.Requests.Users.LoginUser;
using Web.Application.Requests.Users.LogoutUser;
using Web.Application.Requests.Users.RefreshToken;
using Web.Application.Requests.Users.RegisterUser;

/// <summary>
/// API controller for user account operations, such as registration and login.
/// </summary>
public sealed class UserAccountController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<LoginUserRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var request = new LogoutUserRequest(
            UserAccountId: this.User.FindFirstValue("identifier")!);

        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.NoContent();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new RefreshTokenRequest(
            UserAccountId: this.User.FindFirstValue("identifier")!,
            RefreshToken: requestDto.RefreshToken);

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<RegisterUserRequest>(requestDto);
        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.NoContent();
    }
}
