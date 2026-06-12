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
    public void ConstructorShouldThrowArgumentNullExceptionWhenAuthenticatorIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(this.logger, null, this.messageHandler));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(null, this.userSessionContext, this.messageHandler));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenMessageHandlerIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(this.logger, this.userSessionContext, null));
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<GatewayJsonRpc>>();
        this.messageHandler = Substitute.For<IJsonRpcMessageHandler>();
        this.userSessionContext = Substitute.For<IUserSessionContext>();

        this.rpc = new GatewayJsonRpc(this.logger, this.userSessionContext, this.messageHandler);
    }

    [TearDown]
    public void TearDown()
    {
        this.userSessionContext.Dispose();
        this.rpc.Dispose();
    }
}
