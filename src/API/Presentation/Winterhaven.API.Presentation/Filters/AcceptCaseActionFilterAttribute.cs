namespace Winterhaven.API.Presentation.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text.Json;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal sealed class AcceptCaseActionFilterAttribute : ActionFilterAttribute
{
    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Request.Headers["Accept-Case"] == "snake_case" && context.Result is ObjectResult result)
        {
            context.Result = new JsonResult(result.Value, SnakeCaseOptions);
        }

        base.OnActionExecuted(context);
    }
}