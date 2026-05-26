using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Infrastructure.Options.Maps;
using Winterhaven.API.Infrastructure.Services.Maps;

namespace Winterhaven.API.Tests.Infrastructure.Services.Maps;

[TestFixture]
internal sealed class MapLocatorTests
{
    private IFileSystem fileSystem;

    private MapLocator locator;

    private ILogger<MapLocator> logger;

    private IOptions<MapStorageOptions> options;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenFileSystemIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new MapLocator(logger, null, options));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new MapLocator(null, fileSystem, options));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenOptionsIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new MapLocator(logger, fileSystem, null));

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeFileExistsOnceWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        const string expectedData = "data";
        string fullPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(fullPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedData);

        // Act
        await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        fileSystem.File.Received(1).Exists(fullPath);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeFileExistsOnceWhenFileIsEmpty()
    {
        // Arrange
        const string name = "testmap";
        string expectedPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(expectedPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(expectedPath, Arg.Any<CancellationToken>()).Returns("data");

        // Act
        try
        {
            await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
        }

        // Assert
        fileSystem.File.Received(1).Exists(expectedPath);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeFileExistsWithCorrectFullPath()
    {
        // Arrange
        const string name = "testmap";
        // For support for both windows and linux (helpful when debugging on windows instead of linux docker container)
        string expectedPath = Path.Combine(options.Value.BasePath, $"{name.ToLowerInvariant()}.tmx");
        fileSystem.File.Exists(expectedPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(expectedPath, Arg.Any<CancellationToken>()).Returns("data");

        // Act
        await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        fileSystem.File.Received(1).Exists(expectedPath);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeReadAllTextAsyncOnceWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        string fullPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(fullPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(fullPath, Arg.Any<CancellationToken>()).Returns("data");

        // Act
        await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        await fileSystem.File.Received(1).ReadAllTextAsync(fullPath, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeReadAllTextAsyncWithCorrectFullPath()
    {
        // Arrange
        const string name = "testmap";

        // For support for both windows and linux (helpful when debugging on windows instead of linux docker container)
        string expectedPath = Path.Combine(options.Value.BasePath, $"{name.ToLowerInvariant()}.tmx");
        fileSystem.File.Exists(expectedPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(expectedPath, Arg.Any<CancellationToken>()).Returns("data");

        // Act
        await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        await fileSystem.File.Received(1).ReadAllTextAsync(expectedPath, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldNotInvokeReadAllTextAsyncWhenFileDoesNotExist()
    {
        // Arrange
        fileSystem.File.Exists(Arg.Any<string>()).Returns(false);

        // Act
        try
        {
            await locator.LocateMapDataAsync("testmap", default).ConfigureAwait(false);
        }
        catch (ResourceNotFoundException)
        {
        }

        // Assert
        await fileSystem.File.DidNotReceive().ReadAllTextAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldReturnMapDataWithCorrectBytesWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        const string expectedData = "data";
        string fullPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(fullPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedData);

        // Act
        var result = await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedData));
    }

    [Test]
    public async Task LocateMapDataAsyncShouldReturnMapDataWithUppercasedNameWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        const string expectedData = "data";
        string fullPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(fullPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedData);

        // Act
        var result = await locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        Assert.That(result.Name, Is.EqualTo("TESTMAP"));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowArgumentExceptionWhenNameIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => locator.LocateMapDataAsync(string.Empty, default));

    [Test]
    public void LocateMapDataAsyncShouldThrowArgumentExceptionWhenNameIsWhitespace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => locator.LocateMapDataAsync("   ", default));

    [Test]
    public void LocateMapDataAsyncShouldThrowArgumentNullExceptionWhenNameIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => locator.LocateMapDataAsync(null, default));

    [Test]
    public void LocateMapDataAsyncShouldThrowInvalidOperationExceptionWhenFileIsEmpty()
    {
        // Arrange
        const string name = "testmap";
        string expectedPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(expectedPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(expectedPath, Arg.Any<CancellationToken>()).Returns("");

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => locator.LocateMapDataAsync(name, default));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowInvalidOperationExceptionWhenFileIsWhiteSpace()
    {
        // Arrange
        const string name = "testmap";
        string expectedPath = Path.Combine(options.Value.BasePath, $"{name}.tmx");

        fileSystem.File.Exists(expectedPath).Returns(true);
        fileSystem.File.ReadAllTextAsync(expectedPath, Arg.Any<CancellationToken>()).Returns(" \r\n\t");

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => locator.LocateMapDataAsync(name, default));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowResourceNotFoundExceptionWhenFileDoesNotExist()
    {
        // Arrange
        fileSystem.File.Exists(Arg.Any<string>()).Returns(false);

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => locator.LocateMapDataAsync("testmap", default));
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<MapLocator>>();
        fileSystem = Substitute.For<IFileSystem>();
        options = Substitute.For<IOptions<MapStorageOptions>>();

        options.Value.Returns(new MapStorageOptions()
        {
            BasePath = "/maps",
        });

        locator = new MapLocator(logger, fileSystem, options);
    }
}