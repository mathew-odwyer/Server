// <copyright file="UserAccountController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Controllers.Users;

using System.Diagnostics.CodeAnalysis;
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
[ExcludeFromCodeCoverage]
public sealed class UserAccountController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<LoginUserRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        if (response.IsFailed)
        {
            return this.Unauthorized(response.Errors);
        }

        return this.Ok(response.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var request = new LogoutUserRequest()
        {
            UserAccountId = this.User.FindFirstValue("identifier")!,
        };

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        if (response.IsFailed)
        {
            return this.Unauthorized(response.Errors);
        }

        return this.Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new RefreshTokenRequest()
        {
            UserAccountId = this.User.FindFirstValue("identifier")!,
            RefreshToken = requestDto.RefreshToken,
        };

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        if (response.IsFailed)
        {
            return this.Unauthorized(response.Errors);
        }

        return this.Ok(response.Value);
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="requestDto">
    /// The registration data transfer object containing the user's email address, username, and password.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the outcome of the registration.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<RegisterUserRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        if (response.IsFailed)
        {
            return this.BadRequest(response.Errors);
        }

        return this.Ok();
    }
}
