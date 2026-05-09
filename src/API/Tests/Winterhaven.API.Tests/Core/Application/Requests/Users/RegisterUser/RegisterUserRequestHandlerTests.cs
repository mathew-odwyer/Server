namespace Winterhaven.API.Tests.Core.Application.Requests.Users.RegisterUser;

using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;

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

    private Actor actor;

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenLoggerIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(null, this.userRegistrar, this.unitOfWorkFactory, this.userAccountRepository, this.actorRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserRegistrarIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(this.logger, null, this.unitOfWorkFactory, this.userAccountRepository, this.actorRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUnitOfWorkFactoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(this.logger, this.userRegistrar, null, this.userAccountRepository, this.actorRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenUserAccountRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(this.logger, this.userRegistrar, this.unitOfWorkFactory, null, this.actorRepository));
    }

    [Test]
    public void ConstructorShouldThrowArgumentNullExceptionWhenActorRepositoryIsNull()
    {
        // Act and assert
        Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestHandler(this.logger, this.userRegistrar, this.unitOfWorkFactory, this.userAccountRepository, null));
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
            EmailAddress: this.userAccount.EmailAddress,
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userRegistrar.Received(1).RegisterUserAsync(request.EmailAddress, request.Username, request.Password);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkSaveAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: this.userAccount.EmailAddress,
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        this.unitOfWork.Received(1).SaveAsync(default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUnitOfWorkFactoryCreateUnitOfWorkWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: this.userAccount.EmailAddress,
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

        // Assert
        this.unitOfWorkFactory.Received(1).CreateUnitOfWork();
    }

    [Test]
    public async Task HandleShouldInvokeActorRepositoryAddAsyncWhenRequestIsNotNullAndUserIsRegistered()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: this.userAccount.EmailAddress,
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        // TODO: Fix Args.Any to check if the details are the same.
        this.actorRepository.Received(1).AddAsync(Arg.Any<Actor>(), default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [Test]
    public async Task HandleShouldInvokeUserAccountRepositoryAddAsyncWhenRequestIsNotNull()
    {
        // Arrange
        var request = new RegisterUserRequest(
            EmailAddress: this.userAccount.EmailAddress,
            Username: this.userAccount.Username,
            Password: "password");

        // Act
        await this.handler.Handle(request, default).ConfigureAwait(false);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Assert
        this.userAccountRepository.Received(1).AddAsync(this.userAccount, default);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    [SetUp]
    public void Setup()
    {
        this.logger = Substitute.For<ILogger<RegisterUserRequestHandler>>();
        this.userRegistrar = Substitute.For<IUserRegistrar>();
        this.unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        this.userAccountRepository = Substitute.For<IUserAccountRepository>();
        this.actorRepository = Substitute.For<IActorRepository>();
        this.unitOfWork = Substitute.For<IUnitOfWork>();

        this.actor = new Actor()
        {
            Name = "username",
            Type = ActorType.User,
        };

        this.userAccount = new UserAccount()
        {
            EmailAddress = "test@email.com",
            Username = "username",
            Player = new Player()
            {
                Name = "player",
            },
        };

        this.unitOfWorkFactory.CreateUnitOfWork().Returns(this.unitOfWork);
        this.userRegistrar.RegisterUserAsync(this.userAccount.EmailAddress, this.userAccount.Username, "password").Returns(this.userAccount);

        this.handler = new RegisterUserRequestHandler(this.logger, this.userRegistrar, this.unitOfWorkFactory, this.userAccountRepository, this.actorRepository);
    }
}