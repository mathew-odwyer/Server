// <copyright file="UserAccountController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Controllers.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTOs.Users;
using Web.Application.Requests.Users.LoginUser;
using Web.Application.Requests.Users.LogoutUser;
using Web.Application.Requests.Users.RefreshToken;
using Web.Application.Requests.Users.RegisterUser;

/// <summary>
/// Provides API endpoints for managing user account operations, including registration, login, logout, and token refresh.
/// </summary>
public sealed class UserAccountController : ApiControllerBase
{
    /// <summary>
    /// Authenticates a user using the provided login credentials.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="LoginUserRequestDto"/> containing the user's login credentials.
    /// <para>Validation rules:</para>
    /// <list type="bullet">
    ///     <item>
    ///         <description><b>Username:</b> Must not be <c>null</c>, empty, or consist only of whitespace.</description>
    ///     </item>
    ///     <item>
    ///         <description>Must be between 3 and 12 characters in length.</description>
    ///     </item>
    ///     <item>
    ///         <description>May only contain alphanumeric characters, hyphens (<c>-</c>), or underscores (<c>_</c>).</description>
    ///     </item>
    ///     <item>
    ///         <description><b>Password:</b> Must not be <c>null</c> or empty.</description>
    ///     </item>
    ///     <item>
    ///         <description>Must be at least 12 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.</description>
    ///     </item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 200 (OK) response with the generated client token upon successful login.
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
    /// Returns an <see cref="IActionResult"/> containing an HTTP 204 (No Content) response when logout succeeds.
    /// </returns>
    /// <remarks>
    /// Validation rules:
    /// <list type="bullet">
    ///     <item>
    ///         <description><b>UserAccountId:</b> Must not be <c>null</c> or empty.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    [HttpPost]
    [Authorize]
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
    /// Specifies a <see cref="RefreshTokenRequestDto"/> containing the refresh token used to request a new JWT.
    /// <para>Validation rules:</para>
    /// <list type="bullet">
    ///     <item>
    ///         <description><b>UserAccountId:</b> Must not be <c>null</c> or empty.</description>
    ///     </item>
    ///     <item>
    ///         <description><b>RefreshToken:</b> Must not be <c>null</c> or empty.</description>
    ///     </item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 200 (OK) response with the refreshed authentication token.
    /// </returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new RefreshTokenRequest(
            RefreshToken: requestDto.RefreshToken);

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);
        return this.Ok(response);
    }

    /// <summary>
    /// Registers a new user account using the provided registration details.
    /// </summary>
    /// <param name="requestDto">
    /// Specifies a <see cref="RegisterUserRequestDto"/> containing the new user's registration information.
    /// <para>Validation rules:</para>
    /// <list type="bullet">
    ///     <item>
    ///         <description><b>EmailAddress:</b> Must not be <c>null</c> or empty, and must be a valid email address format (e.g., <c>name@example.com</c>).</description>
    ///     </item>
    ///     <item>
    ///         <description><b>Username:</b> Must not be <c>null</c>, empty, or consist only of whitespace. Must be between 3 and 12 characters, and may only contain alphanumeric characters, hyphens (<c>-</c>), or underscores (<c>_</c>).</description>
    ///     </item>
    ///     <item>
    ///         <description><b>Password:</b> Must not be <c>null</c> or empty. Must be at least 12 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.</description>
    ///     </item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">
    /// Specifies a <see cref="CancellationToken"/> used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing an HTTP 204 (No Content) response when registration succeeds.
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
