// <copyright file="RegisterUserRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.RegisterUser;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Users.RegisterUser;
using Web.Application.Validators.Users;

[TestFixture]
internal sealed class RegisterUserRequestValidatorTests
{
    private readonly string emailAddress = "user@test.com";

    private readonly string password = "TestPassword48$#";

    private readonly string username = "username";

    private RegisterUserRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new RegisterUserRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("\n\r\t")]
    [TestCase("test")]
    [TestCase("test.com")]
    [TestCase("@")]
    [TestCase("test@")]
    public void ValidateShouldHaveErrorWhenEmailDoesNotMeetAspNetCoreRequirements(string emailAddress)
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: emailAddress,
            Username: this.username,
            Password: this.password);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(nameof(request.EmailAddress));
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenEmailDoesMeetAspNetCoreRequirements()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: this.emailAddress,
            Username: this.username,
            Password: this.password);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(nameof(request.EmailAddress));
    }

    [Test]
    public void ValidatorShouldHavePasswordValidatorForPassword()
    {
        this.validator.ShouldHaveChildValidator(x => x.Password, typeof(PasswordValidator));
    }

    [Test]
    public void ValidatorShouldHaveUsernameValidatorForUsername()
    {
        // Act and assert
        this.validator.ShouldHaveChildValidator(x => x.Username, typeof(UsernameValidator));
    }
}
