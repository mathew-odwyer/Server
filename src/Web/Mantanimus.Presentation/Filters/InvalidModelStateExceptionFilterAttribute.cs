// <copyright file="InvalidModelStateExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

internal sealed class InvalidModelStateExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (!context.ModelState.IsValid)
        {
            var firstErrorEntry = context.ModelState
                .FirstOrDefault(x => x.Value?.Errors.Count > 0);

            string? firstError = firstErrorEntry.Value?.Errors.FirstOrDefault()?.ErrorMessage;
            string fieldName = firstErrorEntry.Key;

            var details = new ProblemDetails()
            {
                Title = "Invalid Model State",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Detail = firstError != null ? $"{fieldName}: {firstError}" : "One or more validation errors occurred.",
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
