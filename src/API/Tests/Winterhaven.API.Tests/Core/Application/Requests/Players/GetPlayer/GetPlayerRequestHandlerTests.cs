namespace Winterhaven.API.Tests.Core.Application.Requests.Players.GetPlayer;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Players.GetPlayer;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.Common.Exceptions;
using Winterhaven.Common.DTOs.Players;

[TestFixture]
internal sealed class GetPlayerRequestHandlerTests
{
    private GetPlayerRequestHandler handler;

    private ILogger<GetPlayerRequestHandler> logger;

    private Player player;

    private GetPlayerRequestDto playerDto;

    private UserAccount userAccount;

    private IActorContext actorContext;

    private IUserAccountRepository userAccountRepository;

    private Actor actor;

    [Test]
    public void ConstructorShouldShouldThrowArgumentNullExceptionWhenActorContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(this.logger, null, this.userAccountRepository));
    }

    [Test]
    public void ConstructorShouldShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(this.logger, this.actorContext, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(null, this.actorContext, this.userAccountRepository));
    }

    [Test]
    public async Task HandleShouldShouldReturnPlayerDtoWhenPlayerIsFetched()
    {
        // Arrange
        var request = new GetPlayerRequest();

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.Name, Is.SameAs(this.playerDto.Name));
            Assert.That(response.X, Is.EqualTo(this.playerDto.X));
            Assert.That(response.Y, Is.EqualTo(this.playerDto.Y));
        }
    }

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountIsNotFound()
    {
        // Arrange
        var request = new GetPlayerRequest();

        this.userAccountRepository
            .GetByIdAsync(this.actor.Id)
            .Returns((UserAccount)null!);

        // Act and assert
        var exception = Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            this.handler.Handle(request, default));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception!.Name, Is.EqualTo(nameof(UserAccount)));
            Assert.That(exception.Key, Is.EqualTo(this.actor.Id));
        }
    }

    [Test]
    public async Task HandleShouldFetchUserAccountUsingActorId()
    {
        // Arrange
        var request = new GetPlayerRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await this.userAccountRepository.Received(1).GetByIdAsync(this.actor.Id).ConfigureAwait(false);
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<GetPlayerRequestHandler>>();
        this.actorContext = Substitute.For<IActorContext>();
        this.userAccountRepository = Substitute.For<IUserAccountRepository>();

        this.actor = new Actor()
        {
            Name = "Player",
            Id = Guid.NewGuid(),
            Type = ActorType.User,
        };

        this.player = new Player()
        {
            Name = "Player",
        };

        this.userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = this.player.Name,
            EmailAddress = "test@email.com",
            Player = this.player,
        };

        this.playerDto = new GetPlayerRequestDto(
            Name: this.player.Name,
            X: this.player.X,
            Y: this.player.Y);

        this.actorContext.Actor.Returns(this.actor);
        this.userAccountRepository.GetByIdAsync(this.actor.Id).Returns(this.userAccount);

        this.handler = new GetPlayerRequestHandler(this.logger, this.actorContext, this.userAccountRepository);
    }
}