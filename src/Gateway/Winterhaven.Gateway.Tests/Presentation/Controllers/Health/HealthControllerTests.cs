using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using Winterhaven.Gateway.Presentation.Controllers.Health;

namespace Winterhaven.Gateway.Tests.Presentation.Controllers.Health;

[TestFixture]
internal sealed class HealthControllerTests
{
    private HealthController controller;

    private FakeTimeProvider timeProvider;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked() =>
        // Act and assert.
        Assert.DoesNotThrow(() => new HealthController(timeProvider));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenTimeProviderIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new HealthController(null));

    [Test]
    public void GetShouldNotReturnNullWhenIinvoked()
    {
        // Act
        var result = controller.Get();

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetShouldReturnHealthinessWhenInvoked()
    {
        // Act
        var result = controller.Get() as OkObjectResult;

        // Assert
        dynamic content = result.Value;

        Assert.That(content.Status, Is.TypeOf<string>());
        Assert.That(content.Status, Is.EqualTo("Healthiness"));
    }

    [Test]
    public void GetShouldReturnOkObjectResultWhenInvoked()
    {
        // Act
        var result = controller.Get();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public void GetShouldReturnTimeStampWhenInvoked()
    {
        // Arrange
        timeProvider.SetUtcNow(DateTime.UtcNow);

        // Act
        var result = controller.Get() as OkObjectResult;

        // Assert
        dynamic content = result.Value;

        Assert.That(content.TimeStamp, Is.TypeOf<DateTimeOffset>());
        Assert.That(content.TimeStamp, Is.EqualTo(timeProvider.GetUtcNow()));
    }

    [SetUp]
    public void Setup()
    {
        timeProvider = new FakeTimeProvider();
        controller = new HealthController(timeProvider);
    }
}
