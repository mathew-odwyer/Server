namespace Winterhaven.API.Tests.Infrastructure.Services.Maps;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Domain.Exceptions;
using Winterhaven.API.Infrastructure.Options.Maps;
using Winterhaven.API.Infrastructure.Services.Maps;

[TestFixture]
internal sealed class MapLocatorTests
{
    private ILogger<MapLocator> logger;

    private IFileSystem fileSystem;

    private MapLocator locator;

    private IOptions<MapStorageOptions> options;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new MapLocator(null, this.fileSystem, this.options));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenFileSystemIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new MapLocator(this.logger, null, this.options));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenOptionsIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new MapLocator(this.logger, this.fileSystem, null));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowArgumentNullExceptionWhenNameIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.locator.LocateMapDataAsync(null, default));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowArgumentExceptionWhenNameIsEmpty()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => this.locator.LocateMapDataAsync(string.Empty, default));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowArgumentExceptionWhenNameIsWhitespace()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => this.locator.LocateMapDataAsync("   ", default));
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowResourceNotFoundExceptionWhenFileDoesNotExist()
    {
        // Arrange
        this.fileSystem.File.Exists(Arg.Any<string>()).Returns(false);

        // Act and assert
        Assert.ThrowsAsync<ResourceNotFoundException>(() => this.locator.LocateMapDataAsync("testmap", default));
    }

    [Test]
    public async Task LocateMapDataAsyncShouldNotInvokeReadAllBytesAsyncWhenFileDoesNotExist()
    {
        // Arrange
        this.fileSystem.File.Exists(Arg.Any<string>()).Returns(false);

        // Act
        try
        {
            await this.locator.LocateMapDataAsync("testmap", default).ConfigureAwait(false);
        }
        catch (ResourceNotFoundException)
        {
        }

        // Assert
        await this.fileSystem.File.DidNotReceive().ReadAllBytesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeFileExistsWithCorrectFullPath()
    {
        // Arrange
        const string name = "testmap";
#pragma warning disable CA1308 // Normalize strings to uppercase
        // For support for both windows and linux (helpful when debugging on windows instead of
        // linux docker container)
        string expectedPath = Path.Combine(this.options.Value.BasePath, $"{name.ToLowerInvariant()}.tmx");
#pragma warning restore CA1308 // Normalize strings to uppercase

        this.fileSystem.File.Exists(expectedPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(expectedPath, Arg.Any<CancellationToken>()).Returns(new byte[] { 1, 2, 3 });

        // Act
        await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        this.fileSystem.File.Received(1).Exists(expectedPath);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeReadAllBytesAsyncWithCorrectFullPath()
    {
        // Arrange
        const string name = "testmap";

#pragma warning disable CA1308 // Normalize strings to uppercase.
        // For support for both windows and linux (helpful when debugging on windows instead of
        // linux docker container)
        string expectedPath = Path.Combine(this.options.Value.BasePath, $"{name.ToLowerInvariant()}.tmx");
#pragma warning restore CA1308 // Normalize strings to uppercase

        this.fileSystem.File.Exists(expectedPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(expectedPath, Arg.Any<CancellationToken>()).Returns([1, 2, 3]);

        // Act
        await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        await this.fileSystem.File.Received(1).ReadAllBytesAsync(expectedPath, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public void LocateMapDataAsyncShouldThrowInvalidOperationExceptionWhenFileIsEmpty()
    {
        // Arrange
        const string name = "testmap";
        string expectedPath = Path.Combine(this.options.Value.BasePath, $"{name}.tmx");

        this.fileSystem.File.Exists(expectedPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(expectedPath, Arg.Any<CancellationToken>()).Returns(Array.Empty<byte>());

        // Act and assert
        Assert.ThrowsAsync<InvalidOperationException>(() => this.locator.LocateMapDataAsync(name, default));
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeFileExistsOnceWhenFileIsEmpty()
    {
        // Arrange
        const string name = "testmap";
        string expectedPath = Path.Combine(this.options.Value.BasePath, $"{name}.tmx");

        this.fileSystem.File.Exists(expectedPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(expectedPath, Arg.Any<CancellationToken>()).Returns(Array.Empty<byte>());

        // Act
        try
        {
            await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
        }

        // Assert
        this.fileSystem.File.Received(1).Exists(expectedPath);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldReturnMapDataWithUppercasedNameWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        byte[] expectedBytes = new byte[] { 1, 2, 3 };
        string fullPath = System.IO.Path.Combine(this.options.Value.BasePath, $"{name}.tmx");

        this.fileSystem.File.Exists(fullPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        var result = await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        Assert.That(result.Name, Is.EqualTo("TESTMAP"));
    }

    [Test]
    public async Task LocateMapDataAsyncShouldReturnMapDataWithCorrectBytesWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        byte[] expectedBytes = [1, 2, 3];
        string fullPath = System.IO.Path.Combine(this.options.Value.BasePath, $"{name}.tmx");

        this.fileSystem.File.Exists(fullPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        var result = await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        Assert.That(result.Data.ToArray(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeFileExistsOnceWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        byte[] expectedBytes = new byte[] { 1, 2, 3 };
        string fullPath = System.IO.Path.Combine(this.options.Value.BasePath, $"{name}.tmx");

        this.fileSystem.File.Exists(fullPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        this.fileSystem.File.Received(1).Exists(fullPath);
    }

    [Test]
    public async Task LocateMapDataAsyncShouldInvokeReadAllBytesAsyncOnceWhenFileExistsAndHasContent()
    {
        // Arrange
        const string name = "testmap";
        byte[] expectedBytes = new byte[] { 1, 2, 3 };
        string fullPath = System.IO.Path.Combine(this.options.Value.BasePath, $"{name}.tmx");

        this.fileSystem.File.Exists(fullPath).Returns(true);
        this.fileSystem.File.ReadAllBytesAsync(fullPath, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        await this.locator.LocateMapDataAsync(name, default).ConfigureAwait(false);

        // Assert
        await this.fileSystem.File.Received(1).ReadAllBytesAsync(fullPath, Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<MapLocator>>();
        this.fileSystem = Substitute.For<IFileSystem>();
        this.options = Substitute.For<IOptions<MapStorageOptions>>();

        this.options.Value.Returns(new MapStorageOptions()
        {
            BasePath = "/maps",
        });

        this.locator = new MapLocator(this.logger, this.fileSystem, this.options);
    }
}