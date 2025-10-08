// <copyright file="DeletePlayerRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.DeletePlayer;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Players.DeletePlayer;

[TestFixture]
internal sealed class DeletePlayerRequestValidatorTests
{
    private DeletePlayerRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new DeletePlayerRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenNameIsNullOrWhitespace(string name)
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: "0",
            Name: name);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenUserAccountIdIsNullOrWhitespace(string userAccountId)
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: userAccountId,
            Name: "Player123");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserAccountId);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenNameIsProvided()
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: "0",
            Name: "Player123");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenUserAccountIdIsProvided()
    {
        // Arrange
        var request = new DeletePlayerRequest(
            UserAccountId: "0",
            Name: "Player123");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserAccountId);
    }
}
