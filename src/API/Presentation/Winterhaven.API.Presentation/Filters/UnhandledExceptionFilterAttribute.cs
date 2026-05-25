namespace Winterhaven.API.Presentation.Filters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
{
    public UnhandledExceptionFilterAttribute(ILogger<UnhandledExceptionFilterAttribute> logger)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ILogger<UnhandledExceptionFilterAttribute> Logger { get; }

    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        this.Logger.LogError(context.Exception, "An unexpected error occurred.");

        var response = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred. Please try again later.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
        };

        context.Result = new ObjectResult(response);
        context.ExceptionHandled = true;
    }
}