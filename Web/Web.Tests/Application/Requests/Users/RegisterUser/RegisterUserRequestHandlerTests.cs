// <copyright file="RegisterUserRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Tests.Application.Requests.Users.RegisterUser;

using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Web.Application.Requests.Users.RegisterUser;
using Web.Application.Services.Users;

[TestFixture]
internal sealed class RegisterUserRequestHandlerTests
{
    private RegisterUserRequestHandler handler;

    private ILogger<RegisterUserRequestHandler> logger;

    private RegisterUserRequest request;

    private IUserAccountService userAccountService;

    [Test]
    public void ConstructorShouldNotThrowExceptionWhenInvoked()
    {
        // Act and assert
        Assert.DoesNotThrow(() => new RegisterUserRequestHandler(this.logger, this.userAccountService));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountServiceIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(this.logger, null));
    }

    [Test]
    public void ConstructorShouldThrowExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(null, this.userAccountService));
    }

    [Test]
    public async Task HandleShouldInvokeUserAccountServiceRegisterUserAsyncOnceWhenInvoked()
    {
        // Arrange
        this.userAccountService.RegisterUserAsync(
            emailAddress: this.request.EmailAddress,
            username: this.request.Username,
            password: this.request.Password)
            .Returns(Result.Fail(string.Empty));

        // Act
        await this.handler.Handle(this.request, default).ConfigureAwait(false);

        // Assert
        await this.userAccountService.Received(1).RegisterUserAsync(this.request.EmailAddress, this.request.Username, this.request.Password).ConfigureAwait(false);
    }

    [TestCase("Error A")]
    [TestCase("Error A", "Error B")]
    [TestCase("First", "Second", "Third")]
    public async Task HandleShouldReturnFailureWithErrorMessagesWhenRegisterUserAsyncReturnsFailure(params string[] errorMessages)
    {
        // Arrange
        this.userAccountService.RegisterUserAsync(
            emailAddress: this.request.EmailAddress,
            username: this.request.Username,
            password: this.request.Password)
            .Returns(Result.Fail(errorMessages));

        // Act
        var response = await this.handler.Handle(this.request, default).ConfigureAwait(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsFailed, Is.True);
            Assert.That(
                response.Errors.Select(x => x.Message),
                Is.EqualTo(errorMessages),
                "Expected error messages should match exactly");
        });
    }

    [Test]
    public async Task HandleShouldReturnOkWhenRegisterUserAsyncReturnsOk()
    {
        // Arrange
        this.userAccountService.RegisterUserAsync(
            emailAddress: this.request.EmailAddress,
            username: this.request.Username,
            password: this.request.Password)
            .Returns(Result.Ok());

        // Act
        var response = await this.handler.Handle(this.request, default).ConfigureAwait(false);

        // Assert
        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null));
    }

    [SetUp]
    public void Setup()
    {
        this.request = new RegisterUserRequest(
            EmailAddress: "user@email.com",
            Username: "user",
            Password: "password");

        this.logger = Substitute.For<ILogger<RegisterUserRequestHandler>>();
        this.userAccountService = Substitute.For<IUserAccountService>();

        this.handler = new RegisterUserRequestHandler(this.logger, this.userAccountService);
    }
}
