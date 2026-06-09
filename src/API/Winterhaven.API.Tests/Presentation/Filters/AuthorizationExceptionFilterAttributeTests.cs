using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Presentation.Filters;
using Winterhaven.Common.Exceptions;

namespace Winterhaven.API.Tests.Presentation.Filters;

[TestFixture]
internal sealed class AuthorizationExceptionFilterAttributeTests
{
    private ActionContext actionContext;

    private ActionDescriptor actionDescriptor;

    private ExceptionContext exceptionContext;

    private AuthorizationExceptionFilterAttribute filterAttribute;

    private HttpContext httpContext;

    private RouteData routeData;

    [Test]
    public void ClassShouldHaveAttributeUsageAttributeAttached()
    {
        // Arrange
        var type = typeof(AuthorizationExceptionFilterAttribute);
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
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new AuthorizationExceptionFilterAttribute());
    }

    [Test]
    public void OnExceptionShouldNotReturnNotFoundObjectResultWhenExceptionHandled()
    {
        // Arrange
        this.exceptionContext.ExceptionHandled = true;

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.Result, Is.Null);
    }

    [Test]
    public void OnExceptionShouldNotReturnNotFoundObjectResultWhenExceptionIsNotNotFoundException()
    {
        // Arrange
        this.exceptionContext.Exception = new InvalidOperationException();

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.Result, Is.Not.TypeOf<AuthorizationException>());
    }

    [Test]
    public void OnExceptionShouldReturnNotFoundObjectResultWhenExceptionIsNotFoundException()
    {
        // Arrange
        const string message = "This is the message.";
        this.exceptionContext.Exception = new AuthorizationException(message);

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        Assert.That(this.exceptionContext.Exception, Is.TypeOf<AuthorizationException>());
    }

    [Test]
    public void OnExceptionShouldReturnProblemDetailsWhenExceptionIsNotFoundException()
    {
        // Arrange
        const string message = "You can't do that lmao.";
        this.exceptionContext.Exception = new AuthorizationException(message);

        // Act
        this.filterAttribute.OnException(this.exceptionContext);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.exceptionContext.Result, Is.TypeOf<ObjectResult>());

            var forbiddenResult = this.exceptionContext.Result as ObjectResult;
            var problemDetails = forbiddenResult.Value as ProblemDetails;

            Assert.That(problemDetails.Type, Is.EqualTo("https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"));
            Assert.That(problemDetails.Title, Is.EqualTo("Authorization Error"));
            Assert.That(problemDetails.Detail, Is.EqualTo(message));
            Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status401Unauthorized));
        }
    }

    [Test]
    public void OnExceptionShouldSetExceptionHandledToTrueWhenExceptionIsNotFoundException()
    {
        // Arrange
        this.exceptionContext.Exception = new AuthorizationException();

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

        this.filterAttribute = new AuthorizationExceptionFilterAttribute();
    }
}
