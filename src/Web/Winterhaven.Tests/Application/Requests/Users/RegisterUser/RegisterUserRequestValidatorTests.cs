namespace Winterhaven.Tests.Application.Requests.Users.RegisterUser;

using FluentValidation.TestHelper;
using Winterhaven.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.Core.Application.Validators.Users;
using NUnit.Framework;

internal class RegisterUserRequestValidatorTests
{
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
            Username: "username",
            Password: "TestPassword48$#");

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
            EmailAddress: "test@email.com",
            Username: "username",
            Password: "TestPassword48$#");

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
