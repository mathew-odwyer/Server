using System;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Presentation.Filters;

namespace Winterhaven.API.Tests.Presentation.Filters;

internal sealed class AcceptCaseActionFilterAttributeTests
{
    private AcceptCaseActionFilterAttribute acceptCaseFilter;

    [Test]
    public void ClassShouldHaveAttributeUsageAttached()
    {
        // Arrange
        var type = typeof(AcceptCaseActionFilterAttribute);

        // Act
        var attribute = type.GetCustomAttribute<AttributeUsageAttribute>();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.ValidOn, Is.EqualTo(AttributeTargets.Class | AttributeTargets.Method));
            Assert.That(attribute.AllowMultiple, Is.False);
            Assert.That(attribute.Inherited, Is.True);
        }
    }

    [Test]
    public void OnActionExecutedLeavesResultUnchangedWithUnrecognisedAcceptCaseValueAndObjectResult()
    {
        // Arrange
        var original = new ObjectResult(new { Value = 1 });
        var context = BuildContext(
            acceptCaseHeader: "camelCase",
            result: original);

        // Act
        acceptCaseFilter.OnActionExecuted(context);

        // Assert
        Assert.That(context.Result, Is.SameAs(original));
    }

    [Test]
    public void OnActionExecutedShouldPreservesOriginalValueWithSnakeCaseHeaderAndObjectResult()
    {
        // Arrange
        var payload = new { FirstName = "Ada" };
        var context = BuildContext(
            acceptCaseHeader: "snake_case",
            result: new ObjectResult(payload));

        // Act
        acceptCaseFilter.OnActionExecuted(context);

        // Assert
        var jsonResult = (JsonResult)context.Result;
        Assert.That(jsonResult.Value, Is.SameAs(payload));
    }

    [Test]
    public void OnActionExecutedShouldReplacesResultWithJsonResultWithSnakeCaseHeaderAndObjectResult()
    {
        // Arrange
        var payload = new { FirstName = "John", LastName = "DoeGuy" };

        var context = BuildContext(
            acceptCaseHeader: "snake_case",
            result: new ObjectResult(payload));

        // Act
        acceptCaseFilter.OnActionExecuted(context);

        // Assert
        Assert.That(context.Result, Is.InstanceOf<JsonResult>());
    }

    [Test]
    public void OnActionExecutedUsesSnakeCaseNamingPolicyWithSnakeCaseHeaderAndObjectResult()
    {
        // Arrange
        var context = BuildContext(
            acceptCaseHeader: "snake_case",
            result: new ObjectResult(new { Dummy = 1 }));

        // Act
        acceptCaseFilter.OnActionExecuted(context);

        // Assert
        var jsonResult = (JsonResult)context.Result;
        var options = jsonResult.SerializerSettings as JsonSerializerOptions;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(options, Is.Not.Null);
            Assert.That(options.PropertyNamingPolicy, Is.EqualTo(JsonNamingPolicy.SnakeCaseLower));
        }
    }

    [Test]
    public void OnActionExecutedWithNoAcceptCaseHeaderLeavesResultUnchangedAndObjectResult()
    {
        // Arrange
        var original = new ObjectResult(new { Value = 1 });
        var context = BuildContext(
            acceptCaseHeader: null,
            result: original);

        // Act
        acceptCaseFilter.OnActionExecuted(context);

        // Assert
        Assert.That(context.Result, Is.SameAs(original));
    }

    [Test]
    public void OnActionExecutedWithSnakeCaseHeaderLeavesResultUnchangedButNonObjectResult()
    {
        // Arrange
        var original = new StatusCodeResult(204);
        var context = BuildContext(
            acceptCaseHeader: "snake_case",
            result: original);

        // Act
        acceptCaseFilter.OnActionExecuted(context);

        // Assert
        Assert.That(context.Result, Is.SameAs(original));
    }

    [SetUp]
    public void SetUp() =>
        // Arrange
        acceptCaseFilter = new AcceptCaseActionFilterAttribute();

    private static ActionExecutedContext BuildContext(string acceptCaseHeader, IActionResult result)
    {
        var httpContext = new DefaultHttpContext();

        if (acceptCaseHeader is not null)
        {
            httpContext.Request.Headers["Accept-Case"] = acceptCaseHeader;
        }

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            Substitute.For<ActionDescriptor>());

        return new ActionExecutedContext(
            actionContext,
            filters: [],
            controller: Substitute.For<object>())
        {
            Result = result
        };
    }
}
