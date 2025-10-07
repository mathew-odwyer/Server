// <copyright file="UpdatePlayerRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.UpdatePlayer;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Players.UpdatePlayer;

[TestFixture]
internal sealed class UpdatePlayerRequestValidatorTests
{
    private UpdatePlayerRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new UpdatePlayerRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenNameIsNullOrWhitespace(string name)
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: "0",
            Name: name,
            X: 0,
            Y: 0);

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
        var request = new UpdatePlayerRequest(
            UserAccountId: userAccountId,
            Name: "Player123",
            X: 0,
            Y: 0);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserAccountId);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenNameIsProvided()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: "0",
            Name: "Player123",
            X: 0,
            Y: 0);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ValidateShouldNotHaveErrorWhenUserAccountIdIsProvided()
    {
        // Arrange
        var request = new UpdatePlayerRequest(
            UserAccountId: "0",
            Name: "Player123",
            X: 0,
            Y: 0);

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserAccountId);
    }
}
