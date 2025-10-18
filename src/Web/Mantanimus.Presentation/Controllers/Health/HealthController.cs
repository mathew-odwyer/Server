// <copyright file="HealthController.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation.Controllers.Health;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides endpoints for health checks of the API.
/// </summary>
/// <seealso cref="ApiControllerBase" />
public sealed class HealthController : ApiControllerBase
{
    /// <summary>
    /// Returns the current health status of the API.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing the health status and current UTC timestamp.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return await Task.Run<IActionResult>(() => this.Ok(new
        {
            Status = "Healthiness",
            TimeStamp = DateTime.UtcNow
        })).ConfigureAwait(false);
    }
}
