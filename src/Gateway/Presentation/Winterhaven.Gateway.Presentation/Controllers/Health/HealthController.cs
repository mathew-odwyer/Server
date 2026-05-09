namespace Winterhaven.Gateway.Presentation.Controllers.Health;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides endpoints for health checks of the gateway.
/// </summary>
/// <seealso cref="Controller"/>
[ApiController]
[Route("api/[controller]/[action]")]
[ExcludeFromCodeCoverage]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the current health status of the gateway.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing the health status and current UTC timestamp.
    /// </returns>
    [HttpGet]
    [DisableRateLimiting]
    public IActionResult Get()
    {
        return this.Ok(new
        {
            Status = "Healthiness",
            TimeStamp = DateTime.UtcNow
        });
    }
}