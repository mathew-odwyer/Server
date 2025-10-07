// <copyright file="GetPlayersRequestValidatorTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.GetPlayers;

using FluentValidation.TestHelper;
using NUnit.Framework;
using Web.Application.Requests.Players.GetPlayers;

[TestFixture]
internal sealed class GetPlayersRequestValidatorTests
{
    private GetPlayersRequestValidator validator;

    [SetUp]
    public void Setup()
    {
        this.validator = new GetPlayersRequestValidator();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ValidateShouldHaveErrorWhenUserAccountIdIsNullOrWhitespace(string userAccountId)
    {
        // Arrange
        var request = new GetPlayersRequest(
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
        var request = new GetPlayersRequest(
            UserAccountId: "0");

        // Act
        var result = this.validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserAccountId);
    }
}
