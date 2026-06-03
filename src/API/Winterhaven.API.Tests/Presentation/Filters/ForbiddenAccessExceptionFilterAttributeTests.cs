using System;
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
internal sealed class ForbiddenAccessExceptionFilterAttributeTests
{
    private ActionContext actionContext;

    private ActionDescriptor actionDescriptor;

    private ExceptionContext exceptionContext;

    private ForbiddenAccessExceptionFilterAttribute filterAttribute;

    private HttpContext httpContext;

    private RouteData routeData;

    [Test]
    public void ClassShouldHaveAttributeUsageAttributeAttached()
    {
        // Arrange
        var type = typeof(ForbiddenAccessExceptionFilterAttribute);
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
        Assert.DoesNotThrow(() => new ForbiddenAccessExceptionFilterAttribute());

    [Test]
    public void OnExceptionShouldNotReturnForbiddenObjectResultWhenExceptionHandled()
    {
        // Arrange
        exceptionContext.ExceptionHandled = true;

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Result, Is.Null);
    }

    [Test]
    public void OnExceptionShouldNotReturnForbiddenObjectResultWhenExceptionIsNotForbiddenException()
    {
        // Arrange
        exceptionContext.Exception = new InvalidOperationException();

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Result, Is.Not.TypeOf<ForbiddenAccessException>());
    }

    [Test]
    public void OnExceptionShouldReturnForbiddenObjectResultWhenExceptionIsForbiddenException()
    {
        // Arrange
        const string message = "This is the message.";
        exceptionContext.Exception = new ForbiddenAccessException(message);

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Exception, Is.TypeOf<ForbiddenAccessException>());
    }

    [Test]
    public void OnExceptionShouldReturnProblemDetailsWhenExceptionIsForbiddenException()
    {
        // Arrange
        const string message = "Access is forbidden.";
        exceptionContext.Exception = new ForbiddenAccessException();

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exceptionContext.Result, Is.TypeOf<ObjectResult>());

            var forbiddenResult = exceptionContext.Result as ObjectResult;
            var problemDetails = forbiddenResult.Value as ProblemDetails;

            Assert.That(problemDetails.Type, Is.EqualTo("https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"));
            Assert.That(problemDetails.Title, Is.EqualTo("Forbidden"));
            Assert.That(problemDetails.Detail, Is.EqualTo(message));
            Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status403Forbidden));
        }
    }

    [Test]
    public void OnExceptionShouldSetExceptionHandledToTrueWhenExceptionIsForbiddenException()
    {
        // Arrange
        exceptionContext.Exception = new ForbiddenAccessException();

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

        filterAttribute = new ForbiddenAccessExceptionFilterAttribute();
    }
}
