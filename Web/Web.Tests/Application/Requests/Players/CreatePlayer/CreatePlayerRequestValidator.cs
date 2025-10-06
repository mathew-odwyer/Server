// <copyright file="CreatePlayerRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.CreatePlayer;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Players.CreatePlayer;
using Web.Application.Validators.Users;

[TestFixture]
internal sealed class CreatePlayerRequestValidatorTests
{
    private CreatePlayerRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new CreatePlayerRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenUserAccountIdIsNullOrWhitespace(string userAccountId)
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: userAccountId,
            Name: "Player123");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserAccountId);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenUserAccountIdIsProvided()
    {
        // Arrange
        var request = new CreatePlayerRequest(
            UserAccountId: "123",
            Name: "Player123");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserAccountId);
    }

    [Test]
    public void ValidatorShouldHaveUsernameValidatorForName()
    {
        // Act and assert
        this.validator.ShouldHaveChildValidator(x => x.Name, typeof(UsernameValidator));
    }
}
