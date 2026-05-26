using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Presentation.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ResourceNotFoundExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is ResourceNotFoundException exception)
        {
            var details = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Resource Not Found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
            };

            context.Result = new NotFoundObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}