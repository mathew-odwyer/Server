namespace Winterhaven.API.Tests.Infrastructure.Contexts;

using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System;
using System.Security.Claims;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Infrastructure.Contexts;

[TestFixture]
internal sealed class ActorContextTests
{
    private ActorContext actorContext;

    private IHttpContextAccessor accessor;
    private IActorRepository actorRepository;

    private Actor actor;

    [Test]
    public void ConstructorShouldAllowNullHttpContextAccessor()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new ActorContext(this.actorRepository, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenActorRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new ActorContext(null, this.accessor));
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenUserIsNull()
    {
        // Arrange
        this.accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = null!,
        });

        // Act
        var actual = this.actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.SystemActor));
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenHttpContextIsNull()
    {
        // Arrange
        this.accessor.HttpContext.Returns((HttpContext)null);

        // Act
        var actual = this.actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.SystemActor));
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

        this.accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = user,
        });

        // Act
        var actual = this.actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.SystemActor));
        this.actorRepository.DidNotReceive().GetById(Arg.Any<Guid>());
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenIdentifierClaimIsMissing()
    {
        // Arrange
        var identity = new ClaimsIdentity([], "TestAuthType");
        var user = new ClaimsPrincipal(identity);

        this.accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = user,
        });

        // Act
        var actual = this.actorContext.Actor;

        // Assert
        Assert.That(actual, Is.EqualTo(Actor.SystemActor));
        this.actorRepository.DidNotReceive().GetById(Arg.Any<Guid>());
    }

    [Test]
    public void ActorShouldReturnActorFromRepositoryWhenUserIsAuthenticatedAndIdentifierIsFound()
    {
        // Arrange
        var expected = this.actor;
        this.accessor.ClearSubstitute();

        var identity = new ClaimsIdentity(
        [
            new Claim("identifier", this.actor.Id.ToString()),
        ], "TestAuthType");

        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = user,
        };

        this.accessor.HttpContext.Returns(httpContext);

        // Act
        var actual = this.actorContext.Actor;

        // Assert
        Assert.That(actual, Is.SameAs(expected));
        this.actorRepository.Received(1).GetById(this.actor.Id);
    }

    [Test]
    public void ActorShouldThrowActorNotFoundExceptionWhenRepositoryReturnsNull()
    {
        // Arrange
        this.actorRepository.GetById(this.actor.Id).Returns((Actor)null);

        var identity = new ClaimsIdentity(
        [
                new Claim("identifier", this.actor.Id.ToString()),
        ], "TestAuthType");

        var user = new ClaimsPrincipal(identity);

        this.accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = user,
        });

        // Act & Assert
        var exception = Assert.Throws<ResourceNotFoundException>(() => _ = this.actorContext.Actor);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception!.Name, Is.EqualTo(nameof(Actor)));
            Assert.That(exception!.Key, Is.EqualTo(this.actor.Id));
        }
    }

    [Test]
    public void ActorShouldReturnSystemActorWhenUserIsNotAuthenticated()
    {
        // Arrange
        var expected = Actor.SystemActor;
        this.accessor.ClearSubstitute();

        var identity = new ClaimsIdentity(
        [
            new Claim("identifier", this.actor.Id.ToString()),
        ]);

        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = user,
        };

        this.accessor.HttpContext.Returns(httpContext);

        // Act
        var actual = this.actorContext.Actor;

        // Assert
        Assert.That(actual, Is.SameAs(expected));
    }

    [SetUp]
    public void Setup()
    {
        this.accessor = Substitute.For<IHttpContextAccessor>();
        this.actorRepository = Substitute.For<IActorRepository>();

        this.actor = new Actor()
        {
            Id = Guid.NewGuid(),
            Type = ActorType.User,
            Name = "Test",
        };

        this.actorRepository.GetById(this.actor.Id).Returns(this.actor);

        this.actorContext = new ActorContext(this.actorRepository, this.accessor);
    }
}