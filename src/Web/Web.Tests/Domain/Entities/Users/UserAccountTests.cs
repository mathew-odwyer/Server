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
    public void PlayerShouldReturnPlayerWhenInvoked()
    {
        // Act
        var actual = this.userAccount.Player;

        // Assert
        Assert.That(actual, Is.Not.Null);
    }

    [SetUp]
    public void Setup()
    {
        this.userAccount = new UserAccount();
    }
}
