namespace Winterhaven.API.Tests.Core.Application.Requests.Maps.GetMap;

using FluentValidation.TestHelper;
using global::Winterhaven.API.Core.Application.Requests.Maps.GetMap;
using NUnit.Framework;

[TestFixture]
internal sealed class GetMapRequestValidatorTests
{
    private GetMapRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new GetMapRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenNameIsNullOrWhitespace(string name)
    {
        // Arrange
        var request = new GetMapRequest(
            Name: name);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenNameIsProvided()
    {
        // Arrange
        var request = new GetMapRequest(
            Name: "world-map");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}