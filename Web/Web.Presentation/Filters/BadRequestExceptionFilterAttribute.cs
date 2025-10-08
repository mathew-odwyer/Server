namespace Web.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Application.Exceptions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class BadRequestExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ExceptionHandled)
        {
            return;
        }

        if (context.Exception is BadRequestException exception)
        {
            var details = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
