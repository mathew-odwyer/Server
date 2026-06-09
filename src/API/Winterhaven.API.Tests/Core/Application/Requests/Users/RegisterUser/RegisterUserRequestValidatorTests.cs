using FluentValidation.TestHelper;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.API.Core.Application.Validators.Users;

namespace Winterhaven.API.Tests.Core.Application.Requests.Users.RegisterUser;

internal sealed class RegisterUserRequestValidatorTests
{
    private RegisterUserRequestValidator validator;

    [SetUp]
    public void Setup() => validator = new RegisterUserRequestValidator();

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
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Email Address");
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
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor("Email Address");
    }

    [Test]
    public void ValidatorShouldHavePasswordValidatorForPassword() => validator.ShouldHaveChildValidator(x => x.Password, typeof(PasswordValidator));

    [Test]
    public void ValidatorShouldHaveUsernameValidatorForUsername() => validator.ShouldHaveChildValidator(x => x.Username, typeof(UsernameValidator));
}
