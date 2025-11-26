// <copyright file="RegisterUserRequestHandlerTests.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Tests.Application.Requests.Users.RegisterUser;

using Winterhaven.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.Core.Application.Services.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
internal sealed class RegisterUserRequestHandlerTests
{
    private RegisterUserRequestHandler handler;

    private ILogger<RegisterUserRequestHandler> logger;

    private IUserRegistrar userRegistrar;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(null, this.userRegistrar));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserRegistrarIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(this.logger, null));
    }

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull()
    {
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.handler.Handle(null, default));
    }

    [Test]
    public async Task HandleShouldInvokeRegisterUserAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: "test@email.com",
            Username: "username",
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userRegistrar.Received(1).RegisterUserAsync(request.EmailAddress, request.Username, request.Password);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<RegisterUserRequestHandler>>();
        this.userRegistrar = Substitute.For<IUserRegistrar>();

        this.handler = new RegisterUserRequestHandler(this.logger, this.userRegistrar);
    }
}
