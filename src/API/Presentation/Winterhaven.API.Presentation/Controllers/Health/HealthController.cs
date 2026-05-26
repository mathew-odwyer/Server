using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Winterhaven.API.Presentation.Controllers.Health;

/// <summary>
///   Provides endpoints for health checks of the gateway service.
/// </summary>
/// <seealso cref="ApiControllerBase"/>
[ExcludeFromCodeCoverage]
public sealed class HealthController : ApiControllerBase
{
    /// <summary>
    ///   Returns the current health status of the API.
    /// </summary>
    /// <returns>
    ///   Returns an <see cref="IActionResult"/> containing the health status and current UTC timestamp.
    /// </returns>
    [HttpGet]
    [DisableRateLimiting]
    public IActionResult Get() => Ok(new
    {
        Status = "Healthiness",
        TimeStamp = DateTime.UtcNow
    });
}