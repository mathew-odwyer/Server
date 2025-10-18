// <copyright file="ApiControllerBase.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation.Controllers;

using AutoMapper;
using MediatR;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides a base class for API controllers with access to MediatR's <see cref="ISender"/>.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// The mapper used for mapping between objects.
    /// </summary>
    private IMapper? mapper;

    /// <summary>
    /// The sender for handling MediatR requests.
    /// </summary>
    private ISender? sender;

    /// <summary>
    /// Gets the mapper used for mapping objects.
    /// </summary>
    /// <value>
    /// The mapper used for mapping objects.
    /// </value>
    protected IMapper Mapper
    {
        get { return this.mapper ??= this.HttpContext.RequestServices.GetRequiredService<IMapper>(); }
    }

    /// <summary>
    ///   Gets the sender for handling MediatR requests.
    /// </summary>
    /// <value>
    ///   The <see cref="ISender"/> used to send requests to the MediatR pipeline.
    /// </value>
    protected ISender Sender
    {
        get { return this.sender ??= this.HttpContext.RequestServices.GetRequiredService<ISender>(); }
    }
}
