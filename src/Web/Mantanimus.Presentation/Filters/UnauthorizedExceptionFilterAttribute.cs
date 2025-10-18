// <copyright file="UnauthorizedExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation.Filters;

using Mantanimus.Core.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class UnauthorizedExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is AuthorizationException exception)
        {
            var details = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                Title = "Unauthorized Error",
                Detail = exception.Message,
                Status = StatusCodes.Status401Unauthorized,
            };

            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
