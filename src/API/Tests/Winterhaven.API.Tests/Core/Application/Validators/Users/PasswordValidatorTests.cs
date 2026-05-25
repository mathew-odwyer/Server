namespace Winterhaven.API.Tests.Core.Application.Validators.Users;

using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Validators.Users;

[TestFixture]
internal sealed class PasswordValidatorTests
{
    private PasswordValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new PasswordValidator();
    }

    [TestCase("ABCDEFG1!HIJ")] // no lowercase
    public void ValidateShouldHaveErrorWhenMissingLowercase(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase("Abcdefg!Hijk")]
    public void ValidateShouldHaveErrorWhenMissingNumber(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase("Abcdefg1Hijk")]
    public void ValidateShouldHaveErrorWhenMissingSpecialCharacter(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase("abcdef1!ghij")]
    public void ValidateShouldHaveErrorWhenMissingUppercase(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase("AbcdefghijkL")]
    public void ValidateShouldHaveErrorWhenMultipleRequirementsMissing(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase(null)]
    [TestCase("")]
    public void ValidateShouldHaveErrorWhenPasswordIsNullOrEmpty(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase("Abc1!def")]
    [TestCase("shortpwd1!")]
    public void ValidateShouldHaveErrorWhenPasswordTooShort(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel { Password = password });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [TestCase("Abcdef1!Ghij")]
    [TestCase("StrongPassword123!")]
    public void ValidateShouldNotHaveErrorForValidPasswords(string password)
    {
        var inline = this.BuildParentValidator();

        var result = inline.TestValidate(new TestModel
        {
            Password = password
        });

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    private InlineValidator<TestModel> BuildParentValidator()
    {
        var inline = new InlineValidator<TestModel>();

        // Ensure null values are caught at parent level, then run the child validator for non-null values
        inline.RuleFor(x => x.Password)
              .NotNull()
              .SetValidator(this.validator);

        return inline;
    }

    private sealed class TestModel
    {
        public string Password { get; set; }
    }
}