// <copyright file="LogoutUserRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.LogoutUser;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Users.LogoutUser;

[TestFixture]
internal sealed class LogoutUserRequestValidatorTests
{
    private LogoutUserRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new LogoutUserRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenUserAccountIdIsNullOrWhitespace(string userAccountId)
    {
        // Arrange
        var request = new LogoutUserRequest(
            UserAccountId: userAccountId);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserAccountId);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenUserAccountIdIsProvided()
    {
        // Arrange
        var request = new LogoutUserRequest(
            UserAccountId: "0");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserAccountId);
    }
}
