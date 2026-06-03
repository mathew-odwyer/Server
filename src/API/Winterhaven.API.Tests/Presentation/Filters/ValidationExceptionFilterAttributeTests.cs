using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Presentation.Filters;

namespace Winterhaven.API.Tests.Presentation.Filters;

[TestFixture]
internal sealed class ValidationExceptionFilterAttributeTests
{
    private ActionContext actionContext;

    private ActionDescriptor actionDescriptor;

    private ExceptionContext exceptionContext;

    private ValidationExceptionFilterAttribute filterAttribute;

    private HttpContext httpContext;

    private RouteData routeData;

    [Test]
    public void ClassShouldHaveAttributeUsageAttributeAttached()
    {
        // Arrange
        var type = typeof(ValidationExceptionFilterAttribute);
        var attribute = type.GetCustomAttribute<AttributeUsageAttribute>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.AllowMultiple, Is.True);
            Assert.That(attribute.Inherited, Is.True);
            Assert.That(attribute.ValidOn, Is.EqualTo(AttributeTargets.Class | AttributeTargets.Method));
        }
    }

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked() =>
        // Act and assert
        Assert.DoesNotThrow(() => new ValidationExceptionFilterAttribute());

    [Test]
    public void OnExceptionShouldNotReturnNotFoundObjectResultWhenExceptionHandled()
    {
        // Arrange
        exceptionContext.ExceptionHandled = true;

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Result, Is.Null);
    }

    [Test]
    public void OnExceptionShouldNotReturnNotFoundObjectResultWhenExceptionIsNotNotFoundException()
    {
        // Arrange
        exceptionContext.Exception = new InvalidOperationException();

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Result, Is.Not.TypeOf<ValidationException>());
    }

    [Test]
    public void OnExceptionShouldReturnNotFoundObjectResultWhenExceptionIsNotFoundException()
    {
        // Arrange
        const string message = "This is the message.";
        exceptionContext.Exception = new ValidationException(message);

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Exception, Is.TypeOf<ValidationException>());
    }

    [Test]
    public void OnExceptionShouldReturnProblemDetailsWhenExceptionIsValidationException()
    {
        // Arrange
        const string message = "A validation error did do it's thing lol.";
        var errors = new Dictionary<string, string[]>
        {
            ["Property1"] = ["Error message 1."],
            ["Property2"] = ["Error message 2."]
        };

        exceptionContext.Exception = new ValidationException(message)
        {
            Errors = errors,
        };

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exceptionContext.Result, Is.TypeOf<BadRequestObjectResult>());

            var badRequestResult = exceptionContext.Result as BadRequestObjectResult;
            var problemDetails = badRequestResult.Value as ValidationProblemDetails;

            Assert.That(problemDetails.Type, Is.EqualTo("https://tools.ietf.org/html/rfc7231#section-6.5.1"));
            Assert.That(problemDetails.Title, Is.EqualTo("Validation Error"));
            Assert.That(problemDetails.Detail, Is.EqualTo(message));
            Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(problemDetails.Errors, Is.EqualTo(errors));
        }
    }

    [Test]
    public void OnExceptionShouldSetExceptionHandledToTrueWhenExceptionIsNotFoundException()
    {
        // Arrange
        exceptionContext.Exception = new ValidationException();

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.ExceptionHandled, Is.True);
    }

    [Test]
    public void OnExceptionShouldThrowArgumentNullExceptionWhenContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => filterAttribute.OnException(null));

    [SetUp]
    public void SetUp()
    {
        httpContext = new DefaultHttpContext();
        routeData = new RouteData();
        actionDescriptor = Substitute.For<ActionDescriptor>();
        actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
        exceptionContext = new ExceptionContext(actionContext, Array.Empty<IFilterMetadata>());

        filterAttribute = new ValidationExceptionFilterAttribute();
    }
}
