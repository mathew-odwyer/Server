// <copyright file="InvalidModelStateExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// A filter attribute that handles invalid model state by returning a 400 Bad Request response with validation details.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class InvalidModelStateExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (!context.ModelState.IsValid)
        {
            var details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
