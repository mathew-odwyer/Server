// <copyright file="PlayerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Domain.Entities.Players;

using NUnit.Framework;
using Web.Domain.Entities.Players;

[TestFixture]
internal sealed class PlayerTests
{
    private readonly string name = "Player";

    private readonly string userAccountId = Guid.NewGuid().ToString();

    private Player player;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new Player()
        {
            Name = this.name,
            UserAccountId = this.userAccountId,
        });
    }

    [Test]
    public void IsDeletedShouldReturnFalseWhenInvoked()
    {
        // Arrange

        // Act
        bool actual = this.player.IsDeleted;

        // Assert
        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsDeletedShouldReturnTrueWhenSetToTrue()
    {
        // Arrange
        const bool expected = true;

        // Act
        this.player.IsDeleted = expected;
        bool actual = this.player.IsDeleted;

        // Assert
        Assert.That(actual, Is.True);
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
        this.player = new Player()
        {
            Name = this.name,
            UserAccountId = this.userAccountId,
        };
    }

    [Test]
    public void UserAccountIdShouldReturnUserAccountIdWhenInvoked()
    {
        // Arrange
        string expected = this.userAccountId;

        // Act
        string actual = this.player.UserAccountId;

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
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
