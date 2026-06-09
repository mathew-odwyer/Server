using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Players.GetPlayer;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.Common.DTOs.Players;

namespace Winterhaven.API.Tests.Core.Application.Requests.Players.GetPlayer;

[TestFixture]
internal sealed class GetPlayerRequestHandlerTests
{
    private Actor actor;

    private IActorContext actorContext;

    private GetPlayerRequestHandler handler;

    private ILogger<GetPlayerRequestHandler> logger;

    private Player player;

    private GetPlayerRequestDto playerDto;

    private UserAccount userAccount;

    private IUserAccountRepository userAccountRepository;

    [Test]
    public void ConstructorShouldShouldThrowArgumentNullExceptionWhenActorContextIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(logger, null, userAccountRepository));

    [Test]
    public void ConstructorShouldShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(logger, actorContext, null));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(null, actorContext, userAccountRepository));

    [Test]
    public async Task HandleShouldFetchUserAccountUsingActorId()
    {
        // Arrange
        var request = new GetPlayerRequest();

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        await userAccountRepository.Received(1).GetByIdAsync(actor.Id).ConfigureAwait(false);
    }

    [Test]
    public async Task HandleShouldShouldReturnPlayerDtoWhenPlayerIsFetched()
    {
        // Arrange
        var request = new GetPlayerRequest();

        // Act
        var response = await handler.Handle(request, default).ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.Name, Is.SameAs(playerDto.Name));
            Assert.That(response.X, Is.EqualTo(playerDto.X));
            Assert.That(response.Y, Is.EqualTo(playerDto.Y));
        }
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, default));

    [Test]
    public void HandleShouldThrowResourceNotFoundExceptionWhenUserAccountIsNotFound()
    {
        // Arrange
        var request = new GetPlayerRequest();

        userAccountRepository
            .GetByIdAsync(actor.Id)
            .Returns((UserAccount)null!);

        // Act and assert
        var exception = Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            handler.Handle(request, default));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception!.Name, Is.EqualTo(nameof(UserAccount)));
            Assert.That(exception.Key, Is.EqualTo(actor.Id));
        }
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<GetPlayerRequestHandler>>();
        actorContext = Substitute.For<IActorContext>();
        userAccountRepository = Substitute.For<IUserAccountRepository>();

        actor = new Actor()
        {
            Name = "Player",
            Id = Guid.NewGuid(),
            Type = ActorType.User,
        };

        player = new Player()
        {
            Name = "Player",
        };

        userAccount = new UserAccount()
        {
            Id = Guid.NewGuid(),
            Username = player.Name,
            EmailAddress = "test@email.com",
            Player = player,
        };

        playerDto = new GetPlayerRequestDto(
            Name: player.Name,
            X: player.X,
            Y: player.Y);

        actorContext.Actor.Returns(actor);
        userAccountRepository.GetByIdAsync(actor.Id).Returns(userAccount);

        handler = new GetPlayerRequestHandler(logger, actorContext, userAccountRepository);
    }
}
