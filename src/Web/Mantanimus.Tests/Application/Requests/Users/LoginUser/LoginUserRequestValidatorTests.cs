// <copyright file="LoginUserRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Tests.Application.Requests.Users.LoginUser;

using FluentValidation.TestHelper;
using Mantanimus.Core.Application.Requests.Users.LoginUser;
using NUnit.Framework;

[TestFixture]
internal sealed class LoginUserRequestValidatorTests
{
    private LoginUserRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new LoginUserRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenPasswordIsNullOrWhitespace(string password)
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: "username",
            Password: password);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenUsernameIsNullOrWhitespace(string username)
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: username,
            Password: "password");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenPasswordIsProvided()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: "username",
            Password: "password");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenUsernameIsProvided()
    {
        // Arrange
        var request = new LoginUserRequest(
            Username: "username",
            Password: "password");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }
}
