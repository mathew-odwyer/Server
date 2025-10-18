// <copyright file="UserAccountController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation.Controllers.Users;

using Mantanimus.Core.Application.DTOs.Users;
using Mantanimus.Core.Application.Requests.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides API endpoints for managing user account operations, including registration, login, logout, and token refresh.
/// </summary>
public sealed class UserAccountController : ApiControllerBase
{
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
