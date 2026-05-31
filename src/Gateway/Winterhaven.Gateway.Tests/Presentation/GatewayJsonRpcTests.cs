using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Gateway.Infrastructure.Services.Users;
using Winterhaven.Gateway.Presentation;

namespace Winterhaven.Gateway.Tests.Presentation;

[TestFixture]
internal sealed class GatewayJsonRpcTests
{
    private ILogger<GatewayJsonRpc> logger;

    private IJsonRpcMessageHandler messageHandler;

    private GatewayJsonRpc rpc;

    private IUserSessionAuthenticator userSessionAuthenticator;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenAuthenticatorIsNull() =>
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(logger, null, messageHandler));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(null, userSessionAuthenticator, messageHandler));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMessageHandlerIsNull() =>
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(logger, userSessionAuthenticator, null));

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<GatewayJsonRpc>>();
        messageHandler = Substitute.For<IJsonRpcMessageHandler>();
        userSessionAuthenticator = Substitute.For<IUserSessionAuthenticator>();

        rpc = new GatewayJsonRpc(logger, userSessionAuthenticator, messageHandler);
    }

    [TearDown]
    public void TearDown() => rpc.Dispose();
}
