using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Gateway.Core.Domain.Exceptions;
using Winterhaven.Gateway.Infrastructure.Pipeline.Factories;

namespace Winterhaven.Gateway.Tests.Infrastructure.Pipeline.Factories;

[TestFixture]
internal sealed class ApiExceptionFactoryTests
{
    private ApiExceptionFactory exceptionFactory;

    private ILogger<ApiExceptionFactory> logger;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ApiExceptionFactory(null));

    [Test]
    public async Task CreateAsyncShouldPreserveProblemDetailInExceptionMessageWhenBodyIsValidJson()
    {
        // Arrange
        const string detail = "A descriptive error from the downstream service.";
        var response = BuildResponse(HttpStatusCode.InternalServerError, new { detail });

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result.Message, Is.EqualTo(detail));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldReturnGenericValidationExceptionWhenResponseIsBadRequestWithNullProblem()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("not valid json", Encoding.UTF8, "application/problem+json")
        };

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<ValidationException>());
        Assert.That(result!.Message, Is.EqualTo("An unexpected error occurred. Please try again later."));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldReturnNullWhenResponseIsSuccessful()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.Null);
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldThrowAuthorizationExceptionWhenResponseIsUnauthorized()
    {
        // Arrange
        const string detail = "You are not authorised to perform this action.";
        var response = BuildResponse(HttpStatusCode.Unauthorized, new { detail });

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<AuthorizationException>());
        Assert.That(result.Message, Is.EqualTo(detail));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldThrowInvalidOperationExceptionWhenResponseIsInternalServerError()
    {
        // Arrange
        const string detail = "Something went wrong on the server.";
        var response = BuildResponse(HttpStatusCode.InternalServerError, new { detail });

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<InvalidOperationException>());
        Assert.That(result.Message, Is.EqualTo(detail));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldThrowInvalidOperationExceptionWhenResponseIsUnrecognisedStatusCode()
    {
        // Arrange
        var response = BuildResponse(HttpStatusCode.ServiceUnavailable, new
        {
            detail = "Service is under maintenance."
        });

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<InvalidOperationException>());
        Assert.That(result.Message, Does.Contain("503"));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldThrowValidationExceptionWhenResponseIsBadRequestWithErrors()
    {
        // Arrange
        var response = BuildResponse(HttpStatusCode.BadRequest, new
        {
            detail = "One or more validation errors occurred.",
            errors = new Dictionary<string, string[]>
            {
                { "Username", ["Username is already taken."] }
            }
        });

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<ValidationException>());
        var validationException = (ValidationException)result;
        Assert.That(validationException.Errors, Contains.Key("Username"));
        Assert.That(validationException.Errors["Username"], Does.Contain("Username is already taken."));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldThrowValidationExceptionWhenResponseIsBadRequestWithNoErrors()
    {
        // Arrange
        const string detail = "The request payload was invalid.";
        var response = BuildResponse(HttpStatusCode.BadRequest, new { detail });

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<ValidationException>());
        Assert.That(result.Message, Is.EqualTo(detail));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldUseGenericDetailWhenBodyIsEmpty()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(string.Empty, Encoding.UTF8, "application/problem+json")
        };

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<InvalidOperationException>());
        Assert.That(result.Message, Is.EqualTo("An unexpected error occurred. Please try again later."));
        response.Dispose();
    }

    [Test]
    public async Task CreateAsyncShouldUseGenericDetailWhenBodyIsNotValidJson()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act
        var result = await exceptionFactory.CreateAsync(response).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.TypeOf<InvalidOperationException>());
        Assert.That(result.Message, Is.EqualTo("An unexpected error occurred. Please try again later."));
        response.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<ApiExceptionFactory>>();
        exceptionFactory = new ApiExceptionFactory(logger);
    }

    private static HttpResponseMessage BuildResponse(HttpStatusCode statusCode, object body) =>
        new(statusCode)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/problem+json")
        };
}
