namespace Winterhaven.API.Presentation.Controllers;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides a base class for API controllers with access to MediatR's <see cref="ISender"/>.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[ExcludeFromCodeCoverage]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the mapper used for mapping objects.
    /// </summary>
    /// <value>The mapper used for mapping objects.</value>
    protected IMapper Mapper
    {
        get { return field ??= this.HttpContext.RequestServices.GetRequiredService<IMapper>(); }
    }

    /// <summary>
    /// Gets the sender for handling MediatR requests.
    /// </summary>
    /// <value>The <see cref="ISender"/> used to send requests to the MediatR pipeline.</value>
    protected ISender Sender
    {
        get { return field ??= this.HttpContext.RequestServices.GetRequiredService<ISender>(); }
    }
}