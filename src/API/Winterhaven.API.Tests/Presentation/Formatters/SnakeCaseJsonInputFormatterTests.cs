using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using Winterhaven.API.Presentation.Formatters;

namespace Winterhaven.API.Tests.Presentation.Formatters;

[TestFixture]
internal sealed class SnakeCaseJsonInputFormatterTests
{
    private SnakeCaseJsonInputFormatter formatter;

    [Test]
    public void CanReadShouldBeCaseSensitiveToContentCaseHeaderValue()
    {
        // Arrange
        var context = CreateInputFormatterContext(body: "{}", contentCaseHeader: "Snake_Case");

        // Act
        bool result = this.formatter.CanRead(context);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CanReadShouldReturnFalseWhenContentCaseHeaderIsMissing()
    {
        // Arrange
        var context = CreateInputFormatterContext(body: "{}", contentCaseHeader: null);

        // Act
        bool result = this.formatter.CanRead(context);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CanReadShouldReturnFalseWhenContentCaseHeaderIsNotSnakeCase()
    {
        // Arrange
        var context = CreateInputFormatterContext(body: "{}", contentCaseHeader: "camelCase");

        // Act
        bool result = this.formatter.CanRead(context);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CanReadShouldReturnTrueWhenContentCaseHeaderIsSnakeCase()
    {
        // Arrange
        var context = CreateInputFormatterContext(body: "{}", contentCaseHeader: "snake_case");

        // Act
        bool result = this.formatter.CanRead(context);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CanReadShouldThrowArgumentNullExceptionWhenContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => this.formatter.CanRead(null));
    }

    [Test]
    public async Task ReadRequestBodyAsyncShouldAddModelStateErrorWhenBodyIsInvalidJson()
    {
        // Arrange
        var context = CreateInputFormatterContext(
            body: "{not valid json",
            contentCaseHeader: "snake_case",
            modelType: typeof(TestModel),
            modelName: "testModel");

        // Act
        await this.formatter.ReadRequestBodyAsync(context, Encoding.UTF8).ConfigureAwait(false);

        // Assert
        Assert.That(context.ModelState.ContainsKey("testModel"), Is.True);
    }

    [Test]
    public async Task ReadRequestBodyAsyncShouldDeserialiseSnakeCasePropertiesOntoModel()
    {
        // Arrange
        var context = CreateInputFormatterContext(
            body: /*lang=json,strict*/ "{\"first_name\":\"Mat\",\"last_name\":\"Example\"}",
            contentCaseHeader: "snake_case",
            modelType: typeof(TestModel));

        // Act
        var result = await this.formatter.ReadRequestBodyAsync(context, Encoding.UTF8).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.HasError, Is.False);
            Assert.That(((TestModel)result.Model!).FirstName, Is.EqualTo("Mat"));
            Assert.That(((TestModel)result.Model!).LastName, Is.EqualTo("Example"));
        }
    }

    [Test]
    public async Task ReadRequestBodyAsyncShouldReturnFailureResultWhenBodyIsInvalidJson()
    {
        // Arrange
        var context = CreateInputFormatterContext(
            body: "{not valid json",
            contentCaseHeader: "snake_case",
            modelType: typeof(TestModel));

        // Act
        var result = await this.formatter.ReadRequestBodyAsync(context, Encoding.UTF8).ConfigureAwait(false);

        // Assert
        Assert.That(result.HasError, Is.True);
    }

    [Test]
    public async Task ReadRequestBodyAsyncShouldReturnNullModelWhenBodyIsJsonNull()
    {
        // Arrange
        var context = CreateInputFormatterContext(
            body: "null",
            contentCaseHeader: "snake_case",
            modelType: typeof(TestModel));

        // Act
        var result = await this.formatter.ReadRequestBodyAsync(context, Encoding.UTF8).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Model, Is.Null);
        }
    }

    [Test]
    public async Task ReadRequestBodyAsyncShouldReturnSuccessResultWhenBodyIsValidJson()
    {
        // Arrange
        var context = CreateInputFormatterContext(
            body: /*lang=json,strict*/ "{\"first_name\":\"Mat\",\"last_name\":\"Example\"}",
            contentCaseHeader: "snake_case",
            modelType: typeof(TestModel));

        // Act
        var result = await this.formatter.ReadRequestBodyAsync(context, Encoding.UTF8).ConfigureAwait(false);

        // Assert
        Assert.That(result.HasError, Is.False);
    }

    [Test]
    public void ReadRequestBodyAsyncShouldThrowArgumentNullExceptionWhenContextIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.formatter.ReadRequestBodyAsync(null, Encoding.UTF8));
    }

    [Test]
    public void ReadRequestBodyAsyncShouldThrowArgumentNullExceptionWhenEncodingIsNull()
    {
        // Arrange
        var context = CreateInputFormatterContext(body: "{}", contentCaseHeader: "snake_case");

        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.formatter.ReadRequestBodyAsync(context, null));
    }

    [SetUp]
    public void Setup()
    {
        this.formatter = new SnakeCaseJsonInputFormatter();
    }

    private static InputFormatterContext CreateInputFormatterContext(
        string body,
        string contentCaseHeader,
        Type modelType = null,
        string modelName = "model")
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

        if (contentCaseHeader is not null)
        {
            httpContext.Request.Headers["Content-Case"] = contentCaseHeader;
        }

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new InputFormatterContext(
            httpContext,
            modelName,
            new ModelStateDictionary(),
            new EmptyModelMetadataProvider().GetMetadataForType(modelType ?? typeof(TestModel)),
            (stream, encoding) => new StreamReader(stream, encoding));
    }

    private sealed class TestModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
