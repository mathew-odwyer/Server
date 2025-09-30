// <copyright file="UserAccountTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Domain.Entities.Users;

using NUnit.Framework;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class UserAccountTests
{
    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new UserAccount());
    }

    [Test]
    public void PlayersShouldBeEmptyWhenInvoked()
    {
        // Arrange
        var players = this.userAccount.Players;

        // Act
        int length = players.Count;

        // Assert
        Assert.That(length, Is.Zero);
    }

    [SetUp]
    public void Setup()
    {
        this.userAccount = new UserAccount();
    }
}
