using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Brokering.Events.Users;

namespace Winterhaven.Brokering.NATS.Tests;

[TestFixture]
internal sealed class NatsMessageBusTests
{
    private INatsConnection connection;

    private ILogger<NatsMessageBus> logger;

    private NatsMessageBus messageBus;

    private CancellationToken capturedToken;

    [SetUp]
    public void SetUp()
    {
        logger = Substitute.For<ILogger<NatsMessageBus>>();
        connection = Substitute.For<INatsConnection>();
        messageBus = new NatsMessageBus(logger, connection);
        capturedToken = default;
    }

    [TearDown]
    public async Task TearDown() =>
        await connection.DisposeAsync().ConfigureAwait(false);

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenConnectionIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new NatsMessageBus(logger, null));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new NatsMessageBus(null, connection));

    [Test]
    public async Task PublishAsyncShouldInvokeNatsConnectionPublishAsyncOnceWhenInvoked()
    {
        const string accessToken = "access-token";

        var notification = new UserLoggedInEvent(
            Identifier: Guid.NewGuid(),
            AccessToken: accessToken);

        var cancellationToken = new CancellationToken();

        // Act
        await messageBus.PublishAsync(notification, cancellationToken).ConfigureAwait(false);

        // Assert
        await connection.Received(1).PublishAsync(
            subject: Arg.Is(nameof(UserLoggedInEvent)),
            data: Arg.Is<UserLoggedInEvent>(x =>
                x.Identifier == notification.Identifier &&
                x.AccessToken == accessToken),
            cancellationToken: Arg.Is(cancellationToken))
            .ConfigureAwait(false);
    }

    [Test]
    public void PublishAsyncShouldThrowArgumentNullExceptionWhenEventIsNull() =>
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            messageBus.PublishAsync<UserLoggedInEvent>(null));

    [Test]
    public void SubscribeAsyncShouldThrowArgumentNullExceptionWhenConsumerIsNull() =>
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            messageBus.SubscribeAsync<UserLoggedInEvent>(null));

    [Test]
    public async Task SubscribeAsyncShouldSubscribeToSubjectNamedAfterMessageType()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();
        channel.Writer.Complete();

        // Act
        await messageBus.SubscribeAsync<UserLoggedInEvent>(
            (_, _) => Task.CompletedTask)
            .ConfigureAwait(false);

        // Assert
        await connection.Received(1)
            .SubscribeCoreAsync<UserLoggedInEvent>(
                subject: Arg.Is(nameof(UserLoggedInEvent)),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Test]
    public async Task SubscribeAsyncShouldPassLinkedTokenToSubscribeCoreAsync()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();
        channel.Writer.Complete();

        using var callerCts = new CancellationTokenSource();

        // Act
        await messageBus.SubscribeAsync<UserLoggedInEvent>(
            (_, _) => Task.CompletedTask,
            callerCts.Token)
            .ConfigureAwait(false);

        // Assert
        Assert.That(capturedToken, Is.Not.EqualTo(callerCts.Token));
    }

    [Test]
    public async Task SubscribeAsyncShouldCancelSubscribeCoreAsyncTokenWhenCallerTokenIsCancelled()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();
        channel.Writer.Complete();

        using var callerCts = new CancellationTokenSource();

        // Act
        await messageBus.SubscribeAsync<UserLoggedInEvent>(
            (_, _) => Task.CompletedTask,
            callerCts.Token)
            .ConfigureAwait(false);

        Assert.That(capturedToken.IsCancellationRequested, Is.False,
            "Token should not be cancelled before caller cancels");

        await callerCts.CancelAsync().ConfigureAwait(false);

        // Assert
        Assert.That(capturedToken.IsCancellationRequested, Is.True,
            "Linked token must be cancelled when the caller's token is cancelled");
    }

    [Test]
    public async Task SubscribeAsyncShouldCancelSubscribeCoreAsyncTokenWhenSubscriptionIsDisposed()
    {
        // Arrange
        SetUpSubscribeCore<UserLoggedInEvent>();

        // Act
        var subscription = await messageBus.SubscribeAsync<UserLoggedInEvent>(
            (_, _) => Task.CompletedTask,
            CancellationToken.None)
            .ConfigureAwait(false);

        Assert.That(capturedToken.IsCancellationRequested, Is.False,
            "Token should not be cancelled before disposal");

        await subscription.DisposeAsync().ConfigureAwait(false);

        // Assert
        Assert.That(capturedToken.IsCancellationRequested, Is.True,
            "Linked token must be cancelled when the subscription is disposed");
    }

    [Test]
    public async Task SubscribeAsyncShouldInvokeConsumerWhenMessageIsReceived()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();

        var received = new List<UserLoggedInEvent>();
        var consumerInvoked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task Consumer(UserLoggedInEvent data, CancellationToken _ = default)
        {
            received.Add(data);
            consumerInvoked.TrySetResult();
            return Task.CompletedTask;
        }

        var expected = new UserLoggedInEvent(Identifier: Guid.NewGuid(), AccessToken: "token");

        // Act
        await messageBus.SubscribeAsync<UserLoggedInEvent>(Consumer).ConfigureAwait(false);

        await channel.Writer.WriteAsync(new NatsMsg<UserLoggedInEvent> { Data = expected })
            .ConfigureAwait(false);

        await consumerInvoked.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        channel.Writer.Complete();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0].Identifier, Is.EqualTo(expected.Identifier));
            Assert.That(received[0].AccessToken, Is.EqualTo(expected.AccessToken));
        }
    }

    [Test]
    public async Task SubscribeAsyncShouldNotInvokeConsumerWhenMessageDataIsNull()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();

        int callCount = 0;
        var sentinelReceived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var sentinel = new UserLoggedInEvent(Identifier: Guid.NewGuid(), AccessToken: "sentinel");

        Task Consumer(UserLoggedInEvent data, CancellationToken _ = default)
        {
            callCount++;
            if (data == sentinel) sentinelReceived.TrySetResult();
            return Task.CompletedTask;
        }

        // Act
        await messageBus.SubscribeAsync<UserLoggedInEvent>(Consumer).ConfigureAwait(false);

        await channel.Writer.WriteAsync(new NatsMsg<UserLoggedInEvent> { Data = null })
            .ConfigureAwait(false);
        await channel.Writer.WriteAsync(new NatsMsg<UserLoggedInEvent> { Data = sentinel })
            .ConfigureAwait(false);

        await sentinelReceived.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        channel.Writer.Complete();

        // Assert
        Assert.That(callCount, Is.EqualTo(1));
    }

    [Test]
    public async Task SubscribeAsyncShouldLogErrorAndContinueWhenConsumerThrowsNonCancellationException()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();

        int callCount = 0;
        var secondMessageReceived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var message = new NatsMsg<UserLoggedInEvent>
        {
            Data = new UserLoggedInEvent(Identifier: Guid.NewGuid(), AccessToken: "token")
        };

        Task Consumer(UserLoggedInEvent _)
        {
            callCount++;
            if (callCount == 1) throw new InvalidOperationException("boom");
            secondMessageReceived.TrySetResult();
            return Task.CompletedTask;
        }

        // Act
        await messageBus.SubscribeAsync<UserLoggedInEvent>((_, _) => Consumer(null)).ConfigureAwait(false);

        await channel.Writer.WriteAsync(message).ConfigureAwait(false);
        await channel.Writer.WriteAsync(message).ConfigureAwait(false);

        // Assert
        await secondMessageReceived.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        channel.Writer.Complete();

        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Is<Exception>(ex => ex is InvalidOperationException),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Test]
    public async Task SubscribeAsyncShouldExitLoopWhenConsumerThrowsOperationCanceledException()
    {
        // Arrange
        var channel = SetUpSubscribeCore<UserLoggedInEvent>();

        var consumerCalled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task Consumer(UserLoggedInEvent _, CancellationToken ct = default)
        {
            consumerCalled.TrySetResult();
            throw new OperationCanceledException(ct);
        }

        // Act
        var subscription = await messageBus.SubscribeAsync<UserLoggedInEvent>(Consumer)
            .ConfigureAwait(false);

        await channel.Writer.WriteAsync(new NatsMsg<UserLoggedInEvent>
        {
            Data = new UserLoggedInEvent(Identifier: Guid.NewGuid(), AccessToken: "token")
        }).ConfigureAwait(false);

        await consumerCalled.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        // DisposeAsync must complete cleanly — the loop already exited via OCE
        await subscription.DisposeAsync().ConfigureAwait(false);

        // Assert — OCE must not be treated as a general error
        logger.DidNotReceive().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    private Channel<NatsMsg<TData>> SetUpSubscribeCore<TData>()
        where TData : class
    {
        var channel = Channel.CreateUnbounded<NatsMsg<TData>>();
        var fakeSubscription = Substitute.For<INatsSub<TData>>();
        fakeSubscription.Msgs.Returns(channel.Reader);

#pragma warning disable CA2012
        connection
            .SubscribeCoreAsync<TData>(
                Arg.Any<string>(),
                cancellationToken: Arg.Do<CancellationToken>(t => capturedToken = t))
            .Returns(_ => ValueTask.FromResult(fakeSubscription));
#pragma warning restore CA2012

        return channel;
    }
}
