using FluentValidation.TestHelper;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Maps.GetMap;

namespace Winterhaven.API.Tests.Core.Application.Requests.Maps.GetMap;

[TestFixture]
internal sealed class GetMapRequestValidatorTests
{
    private GetMapRequestValidator validator;

    [SetUp]
    public void Setup() => validator = new GetMapRequestValidator();

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenNameIsNullOrWhitespace(string name)
    {
        // Arrange
        var request = new GetMapRequest(
            Name: name);

        // Act
        var result = validator.TestValidate(request);

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
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
