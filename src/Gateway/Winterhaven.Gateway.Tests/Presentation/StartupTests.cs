using System;
using NUnit.Framework;
using Winterhaven.Gateway.Presentation;

namespace Winterhaven.Gateway.Tests.Presentation;

[TestFixture]
internal sealed class StartupTests
{
    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenConfigurationIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new Startup(null));
}
