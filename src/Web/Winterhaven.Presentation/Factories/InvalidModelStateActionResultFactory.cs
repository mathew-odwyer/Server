// <copyright file="InvalidModelStateActionResultFactory.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Presentation.Factories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

internal sealed class InvalidModelStateActionResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var error = context.ModelState
            .FirstOrDefault(x => x.Value?.Errors.Count > 0);

        string fieldName = error.Key;
        string? message = error.Value?.Errors.FirstOrDefault()?.ErrorMessage ?? "One or more validation errors occurred.";

        var details = new ProblemDetails
        {
            Title = "Validation Error",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Detail = fieldName != null ? $"{fieldName}: {message}" : message,
        };

        return new BadRequestObjectResult(details);
    }
}
