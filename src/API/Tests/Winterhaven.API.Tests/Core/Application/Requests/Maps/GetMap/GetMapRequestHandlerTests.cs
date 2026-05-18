namespace Winterhaven.API.Tests.Core.Application.Requests.Maps.GetMap;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Requests.Maps.GetMap;
using Winterhaven.API.Core.Application.Services.Maps;
using Winterhaven.API.Core.Domain.ValueObjects.Maps;

[TestFixture]
internal sealed class GetMapRequestHandlerTests
{
    private GetMapRequestHandler handler;

    private ILogger<GetMapRequestHandler> logger;

    private MapData mapData;

    private IMapLocator mapLocator;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetMapRequestHandler(null, this.mapLocator));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMapLocatorIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetMapRequestHandler(this.logger, null));
    }

    [Test]
    public async Task HandleShouldFetchMapUsingRequestName()
    {
        // Arrange
        var request = new GetMapRequest(
            Name: this.mapData.Name);

        // Act
        await this.handler
            .Handle(request, default)
            .ConfigureAwait(false);

        // Assert
        await this.mapLocator
            .Received(1)
            .LocateMapDataAsync(this.mapData.Name, Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task HandleShouldReturnGetMapResponseWhenMapIsFetched()
    {
        // Arrange
        var request = new GetMapRequest(
            Name: this.mapData.Name);

        // Act
        var response = await this.handler
            .Handle(request, default)
            .ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.Name, Is.EqualTo(this.mapData.Name));
            Assert.That(response.Data, Is.EqualTo(this.mapData.Data));
        }
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            this.handler.Handle(null, default));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<GetMapRequestHandler>>();
        this.mapLocator = Substitute.For<IMapLocator>();

        this.mapData = new MapData(
            Name: "world-map",
            Data: new byte[] { 1, 2, 3 });

        this.mapLocator
            .LocateMapDataAsync(this.mapData.Name, Arg.Any<CancellationToken>())
            .Returns(this.mapData);

        this.handler = new GetMapRequestHandler(
            this.logger,
            this.mapLocator);
    }
}