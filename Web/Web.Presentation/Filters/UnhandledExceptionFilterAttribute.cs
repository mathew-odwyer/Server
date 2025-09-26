namespace Web.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Application.Exceptions;

internal sealed class UnhandledExceptionFilterAttribute : IExceptionFilter
{
    private readonly ILogger<UnhandledExceptionFilterAttribute> logger;

    public UnhandledExceptionFilterAttribute(ILogger<UnhandledExceptionFilterAttribute> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        // If this is from the unhandled exception behaviour, don't log the exception twice.
        if (context.Exception is not UnhandledBehaviourException)
        {
            this.logger.LogError(context.Exception, "An unexpected error occurred.");
        }

        var response = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred. Please try again later.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
