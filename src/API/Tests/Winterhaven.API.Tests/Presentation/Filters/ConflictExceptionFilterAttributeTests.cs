using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Reflection;
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
        Type type = typeof(ConflictExceptionFilterAttribute);
        AttributeUsageAttribute attribute = type.GetCustomAttribute<AttributeUsageAttribute>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.AllowMultiple, Is.True);
            Assert.That(attribute.Inherited, Is.True);
            Assert.That(attribute.ValidOn, Is.EqualTo(AttributeTargets.Class | AttributeTargets.Method));
        }
    }

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new ConflictExceptionFilterAttribute());
    }

    [Test]
    public void OnExceptionShouldNotReturnConflictObjectResultWhenExceptionHandled()
    {
        // Arrange
        this.exceptionContext.ExceptionHandled = true;

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.Result, Is.Null);
    }

    [Test]
    public void OnExceptionShouldNotReturnConflictObjectResultWhenExceptionIsNotConflictException()
    {
        // Arrange
        this.exceptionContext.Exception = new InvalidOperationException();

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.Result, Is.Not.TypeOf<ConflictException>());
    }

    [Test]
    public void OnExceptionShouldReturnConflictObjectResultWhenExceptionIsConflictException()
    {
        // Arrange
        const string message = "This is the message.";
        this.exceptionContext.Exception = new ConflictException(message);

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.Exception, Is.TypeOf<ConflictException>());
    }

    [Test]
    public void OnExceptionShouldReturnProblemDetailsWhenExceptionIsConflictException()
    {
        // Arrange
        const string message = "This is the message.";
        this.exceptionContext.Exception = new ConflictException(message);

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.exceptionContext.Result, Is.TypeOf<ConflictObjectResult>());

            var conflictResult = this.exceptionContext.Result as ConflictObjectResult;
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
        this.exceptionContext.Exception = new ConflictException();

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.ExceptionHandled, Is.True);
    }

    [Test]
    public void OnExceptionShouldThrowArgumentNullExceptionWhenContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => this.filterAttribute.OnException(null));
    }

    [SetUp]
    public void SetUp()
    {
        this.httpContext = new DefaultHttpContext();
        this.routeData = new RouteData();
        this.actionDescriptor = Substitute.For<ActionDescriptor>();
        this.actionContext = new ActionContext(this.httpContext, this.routeData, this.actionDescriptor);
        this.exceptionContext = new ExceptionContext(this.actionContext, Array.Empty<IFilterMetadata>());

        this.filterAttribute = new ConflictExceptionFilterAttribute();
    }
}