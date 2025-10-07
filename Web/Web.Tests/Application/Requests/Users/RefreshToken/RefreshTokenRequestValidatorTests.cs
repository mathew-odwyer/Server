// <copyright file="RefreshTokenRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.RefreshToken;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Users.RefreshToken;

[TestFixture]
internal sealed class RefreshTokenRequestValidatorTests
{
    private RefreshTokenRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new RefreshTokenRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenRefreshTokenIsNullOrWhitespace(string refreshToken)
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: refreshToken,
            UserAccountId: "0");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenUserAccountIdIsNullOrWhitespace(string userAccountId)
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: "RefreshToken",
            UserAccountId: userAccountId);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserAccountId);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenRefreshTokenIsProvided()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            UserAccountId: "0",
            RefreshToken: "RefreshToken");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenUserAccountIdIsProvided()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: "RefreshToken",
            UserAccountId: "0");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserAccountId);
    }
}
