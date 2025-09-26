// <copyright file="ValidationExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Application.Exceptions;

/// <summary>
/// A filter attribute that handles validation exceptions by returning a 400 Bad Request response with validation details.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ValidationExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Exception is ValidationException exception)
        {
            var details = new ValidationProblemDetails(exception.Errors ?? new Dictionary<string, string[]>())
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        base.OnException(context);
    }
}
