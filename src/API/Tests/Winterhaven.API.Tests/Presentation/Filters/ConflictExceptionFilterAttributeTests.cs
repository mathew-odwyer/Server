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

internal sealed class ConflictExceptionFilterAttributeTests
{
    private ActionContext actionContext;

    private ActionDescriptor actionDescriptor;

    private ExceptionContext exceptionContext;

    private ConflictExceptionFilterAttribute filterAttribute;

    private HttpContext httpContext;

    private RouteData routeData;

    [Test]
    public void ClassShouldHaveAttributeUsageAttributeAttached()
    {
        // Arrange
        var type = typeof(ConflictExceptionFilterAttribute);
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
        Assert.DoesNotThrow(() => new ConflictExceptionFilterAttribute());

    [Test]
    public void OnExceptionShouldNotReturnConflictObjectResultWhenExceptionHandled()
    {
        // Arrange
        exceptionContext.ExceptionHandled = true;

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Result, Is.Null);
    }

    [Test]
    public void OnExceptionShouldNotReturnConflictObjectResultWhenExceptionIsNotConflictException()
    {
        // Arrange
        exceptionContext.Exception = new InvalidOperationException();

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Result, Is.Not.TypeOf<ConflictException>());
    }

    [Test]
    public void OnExceptionShouldReturnConflictObjectResultWhenExceptionIsConflictException()
    {
        // Arrange
        const string message = "This is the message.";
        exceptionContext.Exception = new ConflictException(message);

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        Assert.That(exceptionContext.Exception, Is.TypeOf<ConflictException>());
    }

    [Test]
    public void OnExceptionShouldReturnProblemDetailsWhenExceptionIsConflictException()
    {
        // Arrange
        const string message = "This is the message.";
        exceptionContext.Exception = new ConflictException(message);

        // Act
        filterAttribute.OnException(exceptionContext);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exceptionContext.Result, Is.TypeOf<ConflictObjectResult>());

            var conflictResult = exceptionContext.Result as ConflictObjectResult;
            var problemDetails = conflictResult.Value as ProblemDetails;

            Assert.That(problemDetails.Type, Is.EqualTo("https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8"));
            Assert.That(problemDetails.Title, Is.EqualTo("Conflict Error"));
            Assert.That(problemDetails.Detail, Is.EqualTo(message));
            Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status409Conflict));
        }
    }

    [Test]
    public void OnExceptionShouldSetExceptionHandledToTrueWhenExceptionIsConflictException()
    {
        // Arrange
        exceptionContext.Exception = new ConflictException();

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

        filterAttribute = new ConflictExceptionFilterAttribute();
    }
}