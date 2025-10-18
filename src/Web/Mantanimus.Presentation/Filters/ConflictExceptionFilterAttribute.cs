// <copyright file="ConflictExceptionFilterAttribute.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Presentation.Filters;

using Mantanimus.Core.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
                Title = "Conflict Error",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
            };

            context.Result = new ConflictObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
