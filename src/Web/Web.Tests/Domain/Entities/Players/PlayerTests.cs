// <copyright file="PlayerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Domain.Entities.Players;

using NUnit.Framework;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class PlayerTests
{
    private readonly string name = "Player";

    private Player player;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new Player()
        {
            Name = this.name,
            UserAccount = this.userAccount,
        });
    }

    [Test]
    public void NameShouldReturnPlayerWhenInvoked()
    {
        // Arrange
        string expected = this.name;

        // Act
        string actual = this.player.Name;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [SetUp]
    public void Setup()
    {
        this.userAccount = new UserAccount();

        this.player = new Player()
        {
            Name = this.name,
            UserAccount = this.userAccount,
        };
    }

    [Test]
    public void UserAccountShouldReturnUserAccountWhenInvoked()
    {
        // Arrange
        var expected = this.userAccount;

        // Act
        var actual = this.player.UserAccount;

        // Assert
        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void XShouldReturnOneWhenSetToOne()
    {
        // Arrange
        const int expected = 1;

        // Act
        this.player.X = expected;
        int actual = this.player.X;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void XShouldReturnZeroWhenInvoked()
    {
        // Arrange
        const int expected = 0;

        // Act
        int actual = this.player.X;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void YShouldReturnOneWhenSetToOne()
    {
        // Arrange
        const int expected = 1;

        // Act
        this.player.Y = expected;
        int actual = this.player.Y;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void YShouldReturnZeroWhenInvoked()
    {
        // Arrange
        const int expected = 0;

        // Act
        int actual = this.player.Y;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}
