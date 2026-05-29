using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Infrastructure.Contexts;

namespace Winterhaven.API.Tests.Infrastructure.Contexts;

[TestFixture]
internal sealed class ActorContextTests
{
    private IHttpContextAccessor accessor;

    private Actor actor;

    private ActorContext actorContext;

    private IActorRepository actorRepository;

    [Test]
    public void ActorShouldReturnActorFromRepositoryWhenUserIsAuthenticatedAndIdentifierIsFound()
    {
        // Arrange
        var expected = actor;
        accessor.ClearSubstitute();

        var identity = new ClaimsIdentity(
        [
            new Claim("identifier", actor.Id.ToString()),
        ], "TestAuthType");

        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = user,
        };

        accessor.HttpContext.Returns(httpContext);

        // Act
        var actual = actorContext.Actor;

        // Assert
        Assert.That(actual, Is.SameAs(expected));
        actorRepository.Received(1).GetById(actor.Id);
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenHttpContextIsNull()
    {
        // Arrange
        accessor.HttpContext.Returns((HttpContext)null);

        // Act
        var actual = actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.GetSystemActor()));
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenIdentifierClaimIsInvalidGuid()
    {
        // Arrange
        var identity = new ClaimsIdentity(
        [
            new Claim("identifier", "not-a-guid"),
        ], "TestAuthType");

        var user = new ClaimsPrincipal(identity);

        accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = user,
        });

        // Act
        var actual = actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.GetSystemActor()));
        actorRepository.DidNotReceive().GetById(Arg.Any<Guid>());
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenIdentifierClaimIsMissing()
    {
        // Arrange
        var identity = new ClaimsIdentity([], "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = user,
        });

        // Act
        var actual = actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.GetSystemActor()));
        actorRepository.DidNotReceive().GetById(Arg.Any<Guid>());
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenUserIsNotAuthenticated()
    {
        // Arrange
        var expected = Actor.GetSystemActor();
        accessor.ClearSubstitute();

        var identity = new ClaimsIdentity(
        [
            new Claim("identifier", actor.Id.ToString()),
        ]);

        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = user,
        };

        accessor.HttpContext.Returns(httpContext);

        // Act
        var actual = actorContext.Actor;

        // Assert
        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenUserIsNull()
    {
        // Arrange
        accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = null!,
        });

        // Act
        var actual = actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.GetSystemActor()));
    }

    [Test]
    public void ActorShouldThrowActorNotFoundExceptionWhenRepositoryReturnsNull()
    {
        // Arrange
        actorRepository.GetById(actor.Id).Returns((Actor)null);

        var identity = new ClaimsIdentity(
        [
                new Claim("identifier", actor.Id.ToString()),
        ], "TestAuthType");

        var user = new ClaimsPrincipal(identity);

        accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = user,
        });

        // Act & Assert
        var exception = Assert.Throws<ResourceNotFoundException>(() => _ = actorContext.Actor);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception!.Name, Is.EqualTo(nameof(Actor)));
            Assert.That(exception!.Key, Is.EqualTo(actor.Id));
        }
    }

    [Test]
    public void ConstructorShouldAllowNullHttpContextAccessor() =>
        // Act & Assert
        Assert.DoesNotThrow(() => new ActorContext(actorRepository, null));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenActorRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ActorContext(null, accessor));

    [SetUp]
    public void Setup()
    {
        accessor = Substitute.For<IHttpContextAccessor>();
        actorRepository = Substitute.For<IActorRepository>();

        actor = new Actor()
        {
            Id = Guid.NewGuid(),
            Type = ActorType.User,
            Name = "Test",
        };

        actorRepository.GetById(actor.Id).Returns(actor);

        actorContext = new ActorContext(actorRepository, accessor);
    }
}