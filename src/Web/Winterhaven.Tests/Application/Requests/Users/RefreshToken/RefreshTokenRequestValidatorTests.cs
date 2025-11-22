// <copyright file="RefreshTokenRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Tests.Application.Requests.Users.RefreshToken;

using FluentValidation.TestHelper;
using Winterhaven.Core.Application.Requests.Users.RefreshToken;
using NUnit.Framework;

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
            RefreshToken: refreshToken);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenRefreshTokenIsProvided()
    {
        // Arrange
        var request = new RefreshTokenRequest(
            RefreshToken: "RefreshToken");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
    }
}
