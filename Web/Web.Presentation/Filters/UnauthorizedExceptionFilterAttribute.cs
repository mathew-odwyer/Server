// <copyright file="UnauthorizedExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Application.Exceptions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class UnauthorizedExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is UnauthorizedException exception)
        {
            var details = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Detail = exception.Message,
                Status = StatusCodes.Status401Unauthorized,
            };

            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
