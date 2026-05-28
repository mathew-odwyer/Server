using System;
using Microsoft.AspNetCore.Mvc;

namespace Winterhaven.Gateway.Presentation.Controllers.Health;

/// <summary>
///   Provides endpoints for health checks of the gateway service.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    ///   Returns the current health status of the API.
    /// </summary>
    /// <returns>
    ///   Returns an <see cref="IActionResult"/> containing the health status and current UTC timestamp.
    /// </returns>
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        Status = "Healthiness",
        TimeStamp = DateTime.UtcNow
    });
}
