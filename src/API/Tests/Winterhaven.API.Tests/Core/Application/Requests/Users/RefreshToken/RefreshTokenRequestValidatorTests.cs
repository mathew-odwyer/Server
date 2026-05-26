using FluentValidation.TestHelper;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Users.RefreshToken;

namespace Winterhaven.API.Tests.Core.Application.Requests.Users.RefreshToken;

[TestFixture]
internal sealed class RefreshTokenRequestValidatorTests
{
    private RefreshTokenRequestValidator validator;

    [SetUp]
    public void Setup() => validator = new RefreshTokenRequestValidator();

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenRefreshTokenIsNullOrWhitespace(string refreshToken)
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: refreshToken);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Refresh Token");
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenRefreshTokenIsProvided()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: "RefreshToken");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Refresh Token");
    }
}