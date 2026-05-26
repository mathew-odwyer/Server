using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Core.Application.Requests.Users.RegisterUser;

/// <summary>
///   Handles <see cref="RegisterUserRequest"/> messages to register new user accounts.
/// </summary>
/// <seealso cref="IRequestHandler{TRequest}"/>
public sealed class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest>
{
    /// <summary>
    ///   The actor repository, used to link the user account to actor for auditing.
    /// </summary>
    private readonly IActorRepository actorRepository;

    /// <summary>
    ///   The logger.
    /// </summary>
    private readonly ILogger<RegisterUserRequestHandler> logger;

    /// <summary>
    ///   The unit of work factory, used to persist the user account.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    ///   The user account repository, used to add the new user account.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    ///   The user registrar, used to register new users.
    /// </summary>
    private readonly IUserRegistrar userRegistrar;

    /// <summary>
    ///   Initializes a new instance of the <see cref="RegisterUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    ///   Specifies an <see cref="ILogger{TCategoryName}"/> that is used to log messages.
    /// </param>
    /// <param name="userRegistrar">
    ///   Specifies an <see cref="IUserRegistrar"/> that is used to register new users.
    /// </param>
    /// <param name="unitOfWorkFactory">
    ///   Specifies an <see cref="IUnitOfWorkFactory"/> that is used to persist the new user to the database.
    /// </param>
    /// <param name="userAccountRepository">
    ///   Specifies an <see cref="IUserAccountRepository"/> that is used to add the new user to the database.
    /// </param>
    /// <param name="actorRepository">
    ///   Specifies an <see cref="IActorRepository"/> that is used to track the user for auditing purposes.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   Thrown if <paramref name="logger"/> or <paramref name="userRegistrar"/> is <c>null</c>.
    /// </exception>
    public RegisterUserRequestHandler(
        ILogger<RegisterUserRequestHandler> logger,
        IUserRegistrar userRegistrar,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountRepository userAccountRepository,
        IActorRepository actorRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userRegistrar = userRegistrar ?? throw new ArgumentNullException(nameof(userRegistrar));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
        this.actorRepository = actorRepository ?? throw new ArgumentNullException(nameof(actorRepository));
    }

    /// <inheritdoc/>
    public async Task Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        logger.LogDebug("Handling user registration for new user: '{Username}'", request.Username);

        var work = unitOfWorkFactory.CreateUnitOfWork();

        var userAccount = await userRegistrar.RegisterUserAsync(
            emailAddress: request.EmailAddress,
            username: request.Username,
            password: request.Password)
            .ConfigureAwait(false);

        var actor = new Actor()
        {
            Id = userAccount.Id,
            Name = userAccount.Username,
            Type = ActorType.User,
        };

        await actorRepository.AddAsync(actor, cancellationToken).ConfigureAwait(false);
        await userAccountRepository.AddAsync(userAccount, cancellationToken).ConfigureAwait(false);

        await work.SaveAsync(cancellationToken).ConfigureAwait(false);

        logger.LogDebug("User registration succeeded for username: {Username}", request.Username);
    }
}