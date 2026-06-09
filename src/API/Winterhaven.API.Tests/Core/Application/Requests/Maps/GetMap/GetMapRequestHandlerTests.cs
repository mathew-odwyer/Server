using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Maps.GetMap;
using Winterhaven.API.Core.Application.Services.Maps;
using Winterhaven.API.Core.Domain.ValueObjects.Maps;

namespace Winterhaven.API.Tests.Core.Application.Requests.Maps.GetMap;

[TestFixture]
internal sealed class GetMapRequestHandlerTests
{
    private GetMapRequestHandler handler;

    private ILogger<GetMapRequestHandler> logger;

    private MapData mapData;

    private IMapLocator mapLocator;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetMapRequestHandler(null, mapLocator));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMapLocatorIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetMapRequestHandler(logger, null));

    [Test]
    public async Task HandleShouldFetchMapUsingRequestName()
    {
        // Arrange
        var request = new GetMapRequest(
            Name: mapData.Name);

        // Act
        await handler
            .Handle(request, default)
            .ConfigureAwait(false);

        // Assert
        await mapLocator
            .Received(1)
            .LocateMapDataAsync(mapData.Name, Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task HandleShouldReturnGetMapResponseWhenMapIsFetched()
    {
        // Arrange
        var request = new GetMapRequest(
            Name: mapData.Name);

        // Act
        var response = await handler
            .Handle(request, default)
            .ConfigureAwait(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(response.Name, Is.EqualTo(mapData.Name));
            Assert.That(response.Data, Is.EqualTo(mapData.Data));
        }
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.Handle(null, default));

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<GetMapRequestHandler>>();
        mapLocator = Substitute.For<IMapLocator>();

        mapData = new MapData(
            Name: "world-map",
            Data: "data");

        mapLocator
            .LocateMapDataAsync(mapData.Name, Arg.Any<CancellationToken>())
            .Returns(mapData);

        handler = new GetMapRequestHandler(
            logger,
            mapLocator);
    }
}
