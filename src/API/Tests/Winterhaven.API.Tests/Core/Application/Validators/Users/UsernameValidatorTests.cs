namespace Winterhaven.API.Tests.Core.Application.Validators.Users;

using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Validators.Users;

[TestFixture]
internal sealed class UsernameValidatorTests
{
    private UsernameValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new UsernameValidator();
    }

    [TestCase("user name")]
    [TestCase("user!name")]
    [TestCase("user@name")]
    [TestCase("user.name")]
    [TestCase("user$name")]
    public void ValidateShouldHaveErrorWhenUsernameContainsIllegalCharacters(string username)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Username = username
        });

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [TestCase(null)]
    [TestCase("")]
    public void ValidateShouldHaveErrorWhenUsernameIsNullOrEmpty(string username)
    {
        // Arrange
        var inline = this.BuildParentValidator();

        // Act
        var result = inline.TestValidate(new TestModel
        {
            Username = username
        });

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Test]
    public void ValidateShouldHaveErrorWhenUsernameTooLong()
    {
        string longUsername = new('a', 13);

        var inline = this.BuildParentValidator();
        var result = inline.TestValidate(new TestModel
        {
            Username = longUsername
        });

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [TestCase("ab")]
    [TestCase("a")]
    public void ValidateShouldHaveErrorWhenUsernameTooShort(string username)
    {
        var inline = this.BuildParentValidator();
        var result = inline.TestValidate(new TestModel
        {
            Username = username
        });

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [TestCase("abc")]
    [TestCase("username")]
    [TestCase("user123")]
    [TestCase("user-name")]
    [TestCase("user_name")]
    [TestCase("aaaaaaaaaaaa")]
    public void ValidateShouldNotHaveErrorForValidUsernames(string username)
    {
        var inline = this.BuildParentValidator();
        var result = inline.TestValidate(new TestModel
        {
            Username = username
        });

        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    private InlineValidator<TestModel> BuildParentValidator()
    {
        var inline = new InlineValidator<TestModel>();

        // Ensure null values are caught at parent level, then run the child validator for non-null values
        inline.RuleFor(x => x.Username)
              .NotNull()
              .SetValidator(this.validator);

        return inline;
    }

    private sealed class TestModel
    {
        public string Username { get; set; }
    }
}