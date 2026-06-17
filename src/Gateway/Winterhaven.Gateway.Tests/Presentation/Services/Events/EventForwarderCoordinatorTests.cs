using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Common.Events;
using Winterhaven.Gateway.Presentation.Services.Events;

namespace Winterhaven.Gateway.Tests.Presentation.Services.Events;

[TestFixture]
internal sealed class EventForwarderCoordinatorTests
{
    private EventForwarderCoordinator coordinator;

    private List<EventForwarderBase> eventForwarders;

    private ILogger<EventForwarderCoordinator> logger;

    private JsonRpc rpc;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenEventForwardersIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new EventForwarderCoordinator(this.logger, null));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new EventForwarderCoordinator(null, this.eventForwarders));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<EventForwarderCoordinator>>();

        this.eventForwarders =
        [
            Substitute.For<EventForwarderBase>(Substitute.For<IMessageBus>()),
            Substitute.For<EventForwarderBase>(Substitute.For<IMessageBus>()),
        ];

        var pipe = new Pipe();
        this.rpc = new JsonRpc(pipe.Writer.AsStream(), pipe.Reader.AsStream());

        this.coordinator = new EventForwarderCoordinator(this.logger, this.eventForwarders);
    }

    [Test]
    public async Task StartAllForwardersAsyncShouldPassCancellationTokenToEachForwarder()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        await this.coordinator.StartAllForwardersAsync(this.rpc, cancellationTokenSource.Token).ConfigureAwait(false);

        // Assert
        foreach (var eventForwarder in this.eventForwarders)
        {
            await eventForwarder.Received(1).StartAsync(this.rpc, cancellationTokenSource.Token).ConfigureAwait(false);
        }
    }

    [Test]
    public async Task StartAllForwardersAsyncShouldStartEachForwarderWithRpcWhenRpcIsNotDisposed()
    {
        // Act
        await this.coordinator.StartAllForwardersAsync(this.rpc).ConfigureAwait(false);

        // Assert
        foreach (var eventForwarder in this.eventForwarders)
        {
            await eventForwarder.Received(1).StartAsync(this.rpc, Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }
    }

    [Test]
    public void StartAllForwardersAsyncShouldThrowArgumentNullExceptionWhenRpcIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.coordinator.StartAllForwardersAsync(null));
    }

    [TearDown]
    public void TearDown()
    {
        this.rpc.Dispose();
    }
}
