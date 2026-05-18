namespace Winterhaven.API.Tests.Core.Domain.Entities.Users;

using NUnit.Framework;
using System;
using Winterhaven.API.Core.Domain.Entities.Users;

[TestFixture]
internal sealed class ActorTests
{
    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new Actor()
        {
            Name = "User",
        });
    }

    [Test]
    public void SystemUniqueIdentifierShouldReturnCorrectIdentifier()
    {
        // Arrange
        var actor = Actor.GetSystemActor();
        var uniqueIdentifier = Guid.Parse("AC892565-1CB6-4AE2-9616-2AA5E2385168");

        // Act
        var identifier = actor.Id;
        string name = actor.Name;
        var type = actor.Type;

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(identifier, Is.EqualTo(uniqueIdentifier));
            Assert.That(name, Is.EqualTo("SYSTEM"));
            Assert.That(type, Is.EqualTo(ActorType.System));
        }
    }
}