namespace Winterhaven.API.Presentation.Controllers.Users;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Requests.Users.LoginUser;
using Winterhaven.API.Core.Application.Requests.Users.LogoutUser;
using Winterhaven.API.Core.Application.Requests.Users.RefreshToken;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.API.Common.DTOs.Users;

/// <summary>
/// Provides API endpoints for managing user account operations, including registration, login,
/// logout, and token refresh.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class UserAccountController : ApiControllerBase
{
    /// <summary>
    /// Authenticates a user using the provided login credentials.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="LoginUserRequestDto"/> containing the user's login credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 200 (OK) response with the
    /// generated client token upon successful login.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<LoginUserRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.Ok(response);
    }

    /// <summary>
    /// Logs out the currently authenticated user by invalidating their active session.
    /// </summary>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 204 (No Content) response when
    /// logout succeeds.
    /// </returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var request = new LogoutUserRequest();

        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.NoContent();
    }

    /// <summary>
    /// Generates a new JSON Web Token (JWT) using the provided refresh token.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="RefreshTokenRequestDto"/> containing the refresh token used to
    /// request a new JWT.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 200 (OK) response with the
    /// refreshed authentication token.
    /// </returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<RefreshTokenRequest>(requestDto);
        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.Ok(response);
    }

    /// <summary>
    /// Registers a new user account using the provided registration details.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="RegisterUserRequestDto"/> containing the new user's registration information.
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 204 (No Content) response when
    /// registration succeeds.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = this.Mapper.Map<RegisterUserRequest>(requestDto);
        await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        return this.NoContent();
    }
}