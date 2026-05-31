using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Tests.Core.Application.Requests.Users.RegisterUser;

[TestFixture]
internal sealed class RegisterUserRequestHandlerTests
{
    private RegisterUserRequestHandler handler;

    private ILogger<RegisterUserRequestHandler> logger;

    private IUserRegistrar userRegistrar;

    private IUserAccountRepository userAccountRepository;

    private IUnitOfWorkFactory unitOfWorkFactory;

    private IActorRepository actorRepository;

    private IUnitOfWork unitOfWork;

    private UserAccount userAccount;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(null, userRegistrar, unitOfWorkFactory, userAccountRepository, actorRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserRegistrarIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(logger, null, unitOfWorkFactory, userAccountRepository, actorRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(logger, userRegistrar, null, userAccountRepository, actorRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(logger, userRegistrar, unitOfWorkFactory, null, actorRepository));

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenActorRepositoryIsNull() =>
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(logger, userRegistrar, unitOfWorkFactory, userAccountRepository, null));

    [Test]
    public void HandleShouldThrowArgumentNullExceptionWhenRequestIsNull() =>
        // Act and assert
        Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, default));

    [Test]
    public async Task HandleShouldInvokeRegisterUserAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: userAccount.EmailAddress,
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userRegistrar.Received(1).RegisterUserAsync(request.EmailAddress, request.Username, request.Password);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: userAccount.EmailAddress,
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkFactoryCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: userAccount.EmailAddress,
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public async Task HandleShouldInvokeActorRepositoryAddAsyncWhenRequestIsNotNullAndUserIsRegistered()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: userAccount.EmailAddress,
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        actorRepository.Received(1).AddAsync(Arg.Any<Actor>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUserAccountRepositoryAddAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: userAccount.EmailAddress,
            Username: userAccount.Username,
            Password: "password");

        // Act
        await handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        userAccountRepository.Received(1).AddAsync(userAccount, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [SetUp]
    public void Setup()
    {
        logger = Substitute.For<ILogger<RegisterUserRequestHandler>>();
        userRegistrar = Substitute.For<IUserRegistrar>();
        unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        userAccountRepository = Substitute.For<IUserAccountRepository>();
        actorRepository = Substitute.For<IActorRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();

        userAccount = new UserAccount()
        {
            EmailAddress = "test@email.com",
            Username = "username",
            Player = new Player()
            {
                Name = "player",
            },
        };

        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);
        userRegistrar.RegisterUserAsync(userAccount.EmailAddress, userAccount.Username, "password").Returns(userAccount);

        handler = new RegisterUserRequestHandler(logger, userRegistrar, unitOfWorkFactory, userAccountRepository, actorRepository);
    }
}