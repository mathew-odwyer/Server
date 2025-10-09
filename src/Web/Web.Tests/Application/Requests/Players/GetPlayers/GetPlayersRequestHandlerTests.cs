// <copyright file="GetPlayersRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Players.GetPlayers;

using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Web.Application.Contexts.Players;
using Web.Application.DTOs.Players;
using Web.Application.Exceptions;
using Web.Application.Requests.Players.GetPlayers;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

[TestFixture]
internal sealed class GetPlayersRequestHandlerTests
{
    private GetPlayersRequestHandler handler;

    private ILogger<GetPlayersRequestHandler> logger;

    private IMapper mapper;

    private IEnumerable<PlayerDto> playerDtos;

    private IPlayerRepository playerRepository;

    private IEnumerable<Player> players;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayersRequestHandler(null, this.mapper, this.playerRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMapperIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayersRequestHandler(this.logger, null, this.playerRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenPlayerRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayersRequestHandler(this.logger, this.mapper, null));
    }

    [Test]
    public async Task HandleShouldInvokeMapperMapWhenPlayersIsNotEmpty()
    {
        // Arrange
        var request = new GetPlayersRequest(
            UserAccountId: this.userAccount.Id);

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.mapper.Received(1).Map<IEnumerable<PlayerDto>>(this.players);
    }

    [Test]
    public async Task HandleShouldReturnPlayersWhenPlayersIsNotEmpty()
    {
        // Arrange
        var request = new GetPlayersRequest(
            UserAccountId: this.userAccount.Id);

        // Act
        var response = await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        Assert.That(response.Players, Is.SameAs(this.playerDtos));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public void HandleShouldThrowEntityNotFoundExceptionWhenPlayersIsEmpty()
    {
        // Arrange
        var request = new GetPlayersRequest(
            UserAccountId: this.userAccount.Id);

        this.playerRepository.GetPlayersByUserAccountId(this.userAccount.Id, default).Returns([]);

        // Act and assert
        Assert.ThrowsAsync<EntityNotFoundException>(() => this.handler.Handle(request, default));
    }

    [Test]
    public void HandleShouldThrowEntityNotFoundExceptionWhenPlayersIsNull()
    {
        // Arrange
        var request = new GetPlayersRequest(
            UserAccountId: this.userAccount.Id);

        this.playerRepository.GetPlayersByUserAccountId(this.userAccount.Id, default).ReturnsNull();

        // Act and assert
        Assert.ThrowsAsync<EntityNotFoundException>(() => this.handler.Handle(request, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<GetPlayersRequestHandler>>();
        this.mapper = Substitute.For<IMapper>();
        this.playerRepository = Substitute.For<IPlayerRepository>();

        this.userAccount = new UserAccount()
        {
            Id = "0",
        };

        this.players =
        [
            new Player()
            {
                Name = "PlayerA",
                UserAccountId = this.userAccount.Id,
            },

            new Player()
            {
                Name = "PlayerB",
                UserAccountId = this.userAccount.Id,
            },
        ];

        this.playerDtos =
        [
            new PlayerDto(
                Name: "PlayerA",
                X: 0,
                Y: 0),

            new PlayerDto(
                Name: "PlayerB",
                X: 0,
                Y: 0),
        ];

        this.playerRepository.GetPlayersByUserAccountId(this.userAccount.Id, default).Returns(this.players);
        this.mapper.Map<IEnumerable<PlayerDto>>(this.players).Returns(this.playerDtos);

        this.handler = new GetPlayersRequestHandler(this.logger, this.mapper, this.playerRepository);
    }
}
