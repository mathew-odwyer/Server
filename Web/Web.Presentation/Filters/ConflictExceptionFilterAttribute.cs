// <copyright file="ConflictExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Application.Exceptions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ConflictExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is ConflictException exception)
        {
            var details = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
            };

            if (exception.Errors is not null)
            {
                // Extend ProblemDetails with a custom field
                context.HttpContext.Items["errors"] = exception.Errors;
            }

            context.Result = new ObjectResult(new
            {
                type = details.Type,
                title = details.Title,
                status = details.Status,
                detail = details.Detail,
                errors = exception.Errors,
            })
            {
                StatusCode = StatusCodes.Status409Conflict
            };

            context.ExceptionHandled = true;
        }
    }
}
