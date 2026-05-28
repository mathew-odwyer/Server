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
    private readonly TimeProvider timeProvider;

    /// <summary>
    ///   Initializes a new instance of the <see cref="HealthController"/> class.
    /// </summary>
    /// <param name="timeProvider">
    ///   The time provider.
    /// </param>
    public HealthController(TimeProvider timeProvider) => this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

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
        TimeStamp = timeProvider.GetUtcNow(),
    });
}
