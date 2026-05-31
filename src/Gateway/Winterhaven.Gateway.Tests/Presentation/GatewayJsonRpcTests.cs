using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation;

namespace Winterhaven.Gateway.Tests.Presentation;

[TestFixture]
internal sealed class GatewayJsonRpcTests
{
    private ILogger<GatewayJsonRpc> logger;

    private IJsonRpcMessageHandler messageHandler;

    private GatewayJsonRpc rpc;

    private IUserSessionContext userSessionContext;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenAuthenticatorIsNull() =>
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(logger, null, messageHandler));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(null, userSessionContext, messageHandler));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMessageHandlerIsNull() =>
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(logger, userSessionContext, null));

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<GatewayJsonRpc>>();
        messageHandler = Substitute.For<IJsonRpcMessageHandler>();
        userSessionContext = Substitute.For<IUserSessionContext>();

        rpc = new GatewayJsonRpc(logger, userSessionContext, messageHandler);
    }

    [TearDown]
    public void TearDown()
    {
        userSessionContext.Dispose();
        rpc.Dispose();
    }
}
