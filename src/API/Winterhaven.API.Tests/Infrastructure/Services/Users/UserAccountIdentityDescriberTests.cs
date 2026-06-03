using NUnit.Framework;
using Winterhaven.API.Infrastructure.Services.Users;

namespace Winterhaven.API.Tests.Infrastructure.Services.Users;

[TestFixture]
internal sealed class UserAccountIdentityDescriberTests
{
    private UserAccountIdentityErrorDescriber describer;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked() =>
        // Act and assert
        Assert.DoesNotThrow(() => new UserAccountIdentityErrorDescriber());

    [Test]
    public void DefaultErrorShouldReturnIdentityErrorWithExpectedValues()
    {
        // Act
        var error = describer.DefaultError();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Error"));
            Assert.That(error.Description, Is.EqualTo("Something went wrong on our end. Please try again in a moment."));
        }
    }

    [Test]
    public void DuplicateEmailShouldReturnIdentityErrorWithExpectedValues()
    {
        // Arrange
        const string email = "user@email.com";

        // Act
        var error = describer.DuplicateEmail(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Email Address"));
            Assert.That(error.Description, Is.EqualTo($"'{email}' is already registered to an account."));
        }
    }

    [Test]
    public void DuplicateUserNameShouldReturnIdentityErrorWithExpectedValues()
    {
        // Arrange
        const string userName = "username";

        // Act
        var error = describer.DuplicateUserName(userName);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Username"));
            Assert.That(error.Description, Is.EqualTo($"'{userName}' is already taken. Please choose another."));
        }
    }

    [Test]
    public void InvalidEmailShouldReturnIdentityErrorWithExpectedValues()
    {
        // Arrange
        const string email = "invalid-email";

        // Act
        var error = describer.InvalidEmail(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Email Address"));
            Assert.That(error.Description, Is.EqualTo($"'{email}' is not a valid email address."));
        }
    }

    [Test]
    public void InvalidUserNameShouldReturnIdentityErrorWithExpectedValues()
    {
        // Arrange
        const string userName = "invalid-username";

        // Act
        var error = describer.InvalidUserName(userName);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Username"));
            Assert.That(error.Description, Is.EqualTo($"'{userName}' is not a valid username."));
        }
    }

    [Test]
    public void PasswordMismatchShouldReturnIdentityErrorWithExpectedValues()
    {
        // Act
        var error = describer.PasswordMismatch();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo("The password you entered is incorrect."));
        }
    }

    [Test]
    public void PasswordRequiresDigitShouldReturnIdentityErrorWithExpectedValues()
    {
        // Act
        var error = describer.PasswordRequiresDigit();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo("Your password must contain at least one number (0–9)."));
        }
    }

    [Test]
    public void PasswordRequiresLowerShouldReturnIdentityErrorWithExpectedValues()
    {
        // Act
        var error = describer.PasswordRequiresLower();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo("Your password must contain at least one lowercase letter (a–z)."));
        }
    }

    [Test]
    public void PasswordRequiresNonAlphanumericShouldReturnIdentityErrorWithExpectedValues()
    {
        // Act
        var error = describer.PasswordRequiresNonAlphanumeric();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo("Your password must contain at least one special character (e.g. !, @, #, $)."));
        }
    }

    [Test]
    public void PasswordRequiresUniqueCharsShouldReturnIdentityErrorWithExpectedValues()
    {
        // Arrange
        const int uniqueChars = 4;

        // Act
        var error = describer.PasswordRequiresUniqueChars(uniqueChars);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo($"Your password must contain at least {uniqueChars} unique characters."));
        }
    }

    [Test]
    public void PasswordRequiresUpperShouldReturnIdentityErrorWithExpectedValues()
    {
        // Act
        var error = describer.PasswordRequiresUpper();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo("Your password must contain at least one uppercase letter (A–Z)."));
        }
    }

    [Test]
    public void PasswordTooShortShouldReturnIdentityErrorWithExpectedValues()
    {
        // Arrange
        const int length = 8;

        // Act
        var error = describer.PasswordTooShort(length);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(error.Code, Is.EqualTo("Password"));
            Assert.That(error.Description, Is.EqualTo($"Your password must be at least {length} characters long."));
        }
    }

    [SetUp]
    public void SetUp() => describer = new UserAccountIdentityErrorDescriber();
}
