using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Winterhaven.API.Presentation.Controllers;

/// <summary>
///   Provides a base class for API controllers with access to MediatR's <see cref="ISender"/>.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[ExcludeFromCodeCoverage]
public abstract class ApiControllerBase : ControllerBase
{
    private IMapper? mapper;

    private ISender? sender;

    /// <summary>
    ///   Gets the mapper used for mapping objects.
    /// </summary>
    /// <value>
    ///   The mapper used for mapping objects.
    /// </value>
    protected IMapper Mapper => mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();

    /// <summary>
    ///   Gets the sender for handling MediatR requests.
    /// </summary>
    /// <value>
    ///   The <see cref="ISender"/> used to send requests to the MediatR pipeline.
    /// </value>
    protected ISender Sender => sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}