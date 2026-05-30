using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;
using Winterhaven.Gateway.Infrastructure.Services.Users;

namespace Winterhaven.Gateway.Tests.Infrastructure.Services.Users;

[TestFixture]
internal sealed class UserAccountServiceTests
{
    private ILogger<UserAccountService> logger;

    private IUserAccountClient userAccountClient;

    private UserAccountService userAccountService;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(null, userAccountClient));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountClientIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new UserAccountService(logger, null));

    [Test]
    public async Task RegisterAsyncShouldCancelWhenUserAccountClientRegusterUserAsyncCancels()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";
        const string emailAddress = "user@email.com";

        // Act
        await userAccountService.RegisterAsync(username, password, emailAddress, new CancellationToken(true)).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).RegisterUserAsync(
            Arg.Is<RegisterUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password &&
                dto.EmailAddress == emailAddress),
            Arg.Is<CancellationToken>(ct => ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public async Task RegisterAsyncShouldInvokeUserAccountServiceRegisterUserAsyncWhenParametersAreNotNullEmptyOrWhiteSpace()
    {
        // Arrange
        const string username = "test-user";
        const string password = "MyCoolPassword";
        const string emailAddress = "user@email.com";

        // Act
        await userAccountService.RegisterAsync(username, password, emailAddress).ConfigureAwait(false);

        // Assert
        await userAccountClient.Received(1).RegisterUserAsync(
            Arg.Is<RegisterUserRequestDto>(dto =>
                dto.Username == username &&
                dto.Password == password &&
                dto.EmailAddress == emailAddress),
            Arg.Is<CancellationToken>(ct => !ct.IsCancellationRequested)
        ).ConfigureAwait(false);
    }

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenEmailAddressIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", "password", string.Empty));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenEmailAddressIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", "password", "\r\t\n "));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenPasswordIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", string.Empty, "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenPasswordIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("username", "\r\t\n ", "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenUsernameIsEmpty() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync(string.Empty, "password", "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentExceptionWhenUsernameIsWhiteSpace() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentException>(() => userAccountService.RegisterAsync("\r\t\n ", "password", "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentNullExceptionWhenEmailAddressIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RegisterAsync("username", "password", null));

    [Test]
    public void RegisterAsyncShouldThrowArgumentNullExceptionWhenPasswordIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RegisterAsync("username", null, "email"));

    [Test]
    public void RegisterAsyncShouldThrowArgumentNullExceptionWhenUsernameIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => userAccountService.RegisterAsync(null, "password", "email"));

    [SetUp]
    public void SetUp()
    {
        // Arrange
        logger = Substitute.For<ILogger<UserAccountService>>();
        userAccountClient = Substitute.For<IUserAccountClient>();
        userAccountService = new UserAccountService(logger, userAccountClient);
    }
}
