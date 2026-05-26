using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Presentation.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ForbiddenAccessExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is ForbiddenAccessException)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                Detail = "Access is forbidden.",
            };

            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}