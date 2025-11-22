// <copyright file="GetPlayerRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Tests.Application.Requests.Players.GetPlayer;

using System;
using System.Threading.Tasks;
using AutoMapper;
using Winterhaven.Core.Application.Contexts.Users;
using Winterhaven.Core.Application.DTOs.Players;
using Winterhaven.Core.Application.Requests.Players.GetPlayer;
using Winterhaven.Core.Domain.Entities.Players;
using Winterhaven.Core.Domain.Entities.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
internal sealed class GetPlayerRequestHandlerTests
{
    private GetPlayerRequestHandler handler;

    private ILogger<GetPlayerRequestHandler> logger;

    private IMapper mapper;

    private Player player;

    private PlayerDto playerDto;

    private UserAccount userAccount;

    private IUserAccountContext userAccountContext;

    [Test]
    public void ConstructorShouldShouldThrowArgumentNullExceptionWhenUserAccountContextIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(this.logger, this.mapper, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(null, this.mapper, this.userAccountContext));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMapperIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new GetPlayerRequestHandler(this.logger, null, this.userAccountContext));
    }

    [Test]
    public async Task HandleShouldInvokeMapperMapWhenPlayerIsFetched()
    {
        // Arrange
        var request = new GetPlayerRequest();

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.mapper.Received(1).Map<PlayerDto>(this.player);
    }

    [Test]
    public async Task HandleShouldShouldReturnPlayerDtoWhenPlayerIsFetched()
    {
        // Arrange
        var request = new GetPlayerRequest();

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

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<GetPlayerRequestHandler>>();
        this.mapper = Substitute.For<IMapper>();
        this.userAccountContext = Substitute.For<IUserAccountContext>();

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

        this.playerDto = new PlayerDto(
            Name: this.player.Name,
            X: this.player.X,
            Y: this.player.Y);

        this.mapper.Map<PlayerDto>(this.player).Returns(this.playerDto);
        this.userAccountContext.User.Returns(this.userAccount);

        this.handler = new GetPlayerRequestHandler(this.logger, this.mapper, this.userAccountContext);
    }
}
