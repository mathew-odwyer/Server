// <copyright file="GetPlayerRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.GetPlayer;

using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Web.Application.Contexts.Players;
using Web.Application.DTOs.Players;
using Web.Application.Exceptions;
using Web.Application.Requests.Players.GetPlayer;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class GetPlayerRequestHandlerTests
{
    private GetPlayerRequestHandler handler;

    private ILogger<GetPlayerRequestHandler> logger;

    private IMapper mapper;

    private Player player;

    private PlayerDto playerDto;

    private IPlayerRepository playerRepository;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(this.logger, this.mapper, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(null, this.mapper, this.playerRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMapperIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(this.logger, null, this.playerRepository));
    }

    [Test]
    public async Task HandleShouldInvokeMapperMapWhenPlayerIsFetched()
    {
        // Arrange
        var request = new GetPlayerRequest(
            PlayerId: this.userAccount.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.mapper.Received(1).Map<PlayerDto>(this.player);
    }

    [Test]
    public async Task HandleShouldShouldReturnPlayerDtoWhenPlayerIsFetched()
    {
        // Arrange
        var request = new GetPlayerRequest(
            PlayerId: this.userAccount.Id);

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(response.Player, Is.SameAs(this.playerDto));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public void HandleShouldThrowEntityNotFoundExceptionWhenGetPlayerByUserAccountIdAsyncReturnsNull()
    {
        // Arrange
        this.playerRepository.ClearSubstitute();

        var request = new GetPlayerRequest(
            PlayerId: this.userAccount.Id);

        // Act and assert
        Assert.ThrowsAsync<EntityNotFoundException>(() => this.handler.Handle(request, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<GetPlayerRequestHandler>>();
        this.mapper = Substitute.For<IMapper>();
        this.playerRepository = Substitute.For<IPlayerRepository>();

        this.userAccount = new UserAccount()
        {
            Id = "0",
        };

        this.player = new Player()
        {
            Name = "Player",
            UserAccount = this.userAccount,
        };

        this.playerDto = new PlayerDto(
            Name: this.player.Name,
            X: this.player.X,
            Y: this.player.Y);

        this.mapper.Map<PlayerDto>(this.player).Returns(this.playerDto);
        this.playerRepository.GetPlayerByUserAccountId(this.player.Name, default).Returns(this.player);

        this.handler = new GetPlayerRequestHandler(this.logger, this.mapper, this.playerRepository);
    }
}
