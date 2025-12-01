// <copyright file="InvalidModelStateExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

internal sealed class InvalidModelStateActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors
                        .Select(x => x.ErrorMessage)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray()
                );

            var details = new ValidationProblemDetails
            {
                Title = "Invalid Model State",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Errors = errors ?? [],
            };

            context.Result = new BadRequestObjectResult(details);
        }
    }
}
