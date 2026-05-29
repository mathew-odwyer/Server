using System;
using NSubstitute;
using NUnit.Framework;
using StreamJsonRpc;
using Winterhaven.Gateway.Presentation;

namespace Winterhaven.Gateway.Tests.Presentation;

[TestFixture]
internal sealed class GatewayJsonRpcTests
{
    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Arrange
        var handler = Substitute.For<IJsonRpcMessageHandler>();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GatewayJsonRpc(null, handler));
    }
}
