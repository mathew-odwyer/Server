namespace Winterhaven.API.Presentation.Filters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.Common.Exceptions;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ValidationExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is ValidationException exception)
        {
            var details = new ValidationProblemDetails
            {
                Title = "Validation Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Errors = exception.Errors ?? new Dictionary<string, string[]>()
                {
                    { "Invalid Model State Error", ["One or more validation errors occurred."] }
                },
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        base.OnException(context);
    }
}